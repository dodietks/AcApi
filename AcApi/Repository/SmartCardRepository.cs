using Microsoft.Extensions.Logging;
using AcApi.Infrastructure;
using System;
using AcApi.Utils;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Generic;
using AcApi.Models;
using System.Threading;

namespace AcApi.Controllers
{
    public class SmartCardRepository
    {
        private readonly ILogger<SmartCardRepository> _logger;
        private DeviceConnection _deviceConnection;
        private readonly ConfigurationOptions _configuration;

        private static string MajorType = "Event";
        private static string MinorType = "INVALID_CARD";

        public SmartCardRepository(ILogger<SmartCardRepository> logger, DeviceConnection deviceConnection, ConfigurationOptions configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _deviceConnection = deviceConnection;
        }

        public SmartCardDTO GetLastCard(Gate gate)
        {
            int _userID = _deviceConnection.DoLogin(gate);
            DateTime now = DateTime.Now;
            var result = GetCards(_userID, now.AddMinutes(-10), now);
            _logger.LogInformation($"I'm in: GetLastCard with Gate ID: {gate.GateId}.");
            SmartCardDTO smartCard = null;
            if (result.Count > 0)
            {
                smartCard = new SmartCardDTO()
                {
                    Id = result[0].CardNumber,
                    GateId = gate.GateId,
                    DateTime = result[0].DateTimeInString
                };
            }
            return smartCard;
        }

        public List<CardRead> GetCards(int loggedUser, DateTime start, DateTime end)
        {

            CHCNetSDK.NET_DVR_ACS_EVENT_COND struCond = new CHCNetSDK.NET_DVR_ACS_EVENT_COND();
            struCond.Init();
            struCond.dwSize = (uint)Marshal.SizeOf(struCond);

            struCond.dwMajor = GetAcsEventType.ReturnMajorTypeValue(ref MajorType);
            struCond.dwMinor = GetAcsEventType.ReturnMinorTypeValue(ref MinorType);

            struCond.struStartTime.dwYear = start.Year;
            struCond.struStartTime.dwMonth = start.Month;
            struCond.struStartTime.dwDay = start.Day;
            struCond.struStartTime.dwHour = start.Hour;
            struCond.struStartTime.dwMinute = start.Minute;
            struCond.struStartTime.dwSecond = start.Second;

            struCond.struEndTime.dwYear = end.Year;
            struCond.struEndTime.dwMonth = end.Month;
            struCond.struEndTime.dwDay = end.Day;
            struCond.struEndTime.dwHour = end.Hour;
            struCond.struEndTime.dwMinute = end.Minute;
            struCond.struEndTime.dwSecond = end.Second;

            struCond.byPicEnable = 0;
            struCond.szMonitorID = "";
            struCond.wInductiveEventType = 65535;

            uint dwSize = struCond.dwSize;
            IntPtr ptrCond = Marshal.AllocHGlobal((int)dwSize);
            Marshal.StructureToPtr(struCond, ptrCond, false);
            int result = CHCNetSDK.NET_DVR_StartRemoteConfig(loggedUser, CHCNetSDK.NET_DVR_GET_ACS_EVENT, ptrCond, (int)dwSize, null, IntPtr.Zero);

            if (-1 == result)
            {
                Marshal.FreeHGlobal(ptrCond);
                _logger.LogInformation("NET_DVR_StartRemoteConfig FAIL, ERROR CODE" + CHCNetSDK.NET_DVR_GetLastError().ToString(), "Error");
                CHCNetSDK.NET_DVR_Logout(loggedUser);
                _logger.LogInformation($"Successful to logout user {loggedUser}");
                return null;
            }
            var list = ProcessEvent(result);
            Marshal.FreeHGlobal(ptrCond);
            CHCNetSDK.NET_DVR_Logout(loggedUser);
            _logger.LogInformation($"Successful to logout user {loggedUser}, {list.Count}");
            return list;
        }

        public List<CardRead> ProcessEvent(int AcsEventHandle)
        {
            int dwStatus = 0;
            List<CardRead> result = new List<CardRead>();
            Boolean Flag = true;
            CHCNetSDK.NET_DVR_ACS_EVENT_CFG struCFG = new CHCNetSDK.NET_DVR_ACS_EVENT_CFG();
            struCFG.dwSize = (uint)Marshal.SizeOf(struCFG);
            int dwOutBuffSize = (int)struCFG.dwSize;
            struCFG.init();
            while (Flag)
            {
                dwStatus = CHCNetSDK.NET_DVR_GetNextRemoteConfig(AcsEventHandle, ref struCFG, dwOutBuffSize);
                switch (dwStatus)
                {
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_SUCCESS://Os dados foram lidos com sucesso，Após processar os dados, chame o proximo.
                        result.Add(ShowCardList(ref struCFG));
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_NEED_WAIT:
                        Thread.Sleep(200);
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_FAILED:
                        CHCNetSDK.NET_DVR_StopRemoteConfig(AcsEventHandle);
                        Flag = false;
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_FINISH:
                        CHCNetSDK.NET_DVR_StopRemoteConfig(AcsEventHandle);
                        Flag = false;
                        break;
                    default:
                        Flag = false;
                        CHCNetSDK.NET_DVR_StopRemoteConfig(AcsEventHandle);
                        break;
                }
            }
            return result;
        }

        private CardRead ShowCardList(ref CHCNetSDK.NET_DVR_ACS_EVENT_CFG struEventCfg)
        {
            CardRead Cards = new CardRead();
            string Number = System.Text.Encoding.UTF8.GetString(struEventCfg.struAcsEventInfo.byCardNo);
            Cards.CardNumber = Number;
            string LogTime = GetStrLogTime(ref struEventCfg.struTime);
            Cards.DateTimeInString = LogTime;

            return Cards;
        }

        private string GetStrLogTime(ref CHCNetSDK.NET_DVR_TIME time)
        {
            string res = time.dwYear.ToString() + ":" + time.dwMonth.ToString() + ":"
                + time.dwDay.ToString() + ":" + time.dwHour.ToString() + ":" + time.dwMinute.ToString()
                + ":" + time.dwSecond.ToString();
            return res;
        }
    }
}
