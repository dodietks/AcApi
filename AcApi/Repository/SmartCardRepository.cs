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

        private static string MajorType = "Event";
        private static string MinorType = "INVALID_CARD";

        public SmartCardRepository(ILogger<SmartCardRepository> logger, DeviceConnection deviceConnection)
        {
            _logger = logger;
            _deviceConnection = deviceConnection;
        }


        public Object GetLastCard()
        {
            int _userID = _deviceConnection.DoLogin();
            Debug.WriteLine("Login efetuado, tentando recuperar eventos de acesso.");

            DateTime now = DateTime.Now;
            var result = GetCards(_userID, now.AddMinutes(-10), now);
            Debug.WriteLine($"Estou em: GetLastCard, Resultado: {0}", result);
            
            return result;
        }

        public object GetCards(int loggedUser, DateTime start, DateTime end)
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

            Debug.WriteLine(ptrCond);
            Debug.WriteLine(struCond);
            Debug.WriteLine(result);
            if (-1 == result)
            {
                Marshal.FreeHGlobal(ptrCond);
                Debug.WriteLine("NET_DVR_StartRemoteConfig FAIL, ERROR CODE" + CHCNetSDK.NET_DVR_GetLastError().ToString(), "Error");
                CHCNetSDK.NET_DVR_Logout(loggedUser);
                Debug.WriteLine($"Successful to logout user {0} ", loggedUser);
                return null;
            }
            var list = ProcessEvent(result);
            Marshal.FreeHGlobal(ptrCond);
            CHCNetSDK.NET_DVR_Logout(loggedUser);
            Debug.WriteLine($"Successful to logout user {0} ", loggedUser);
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
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_SUCCESS://Os dados foram lidos com sucesso，Após processar os dados, chame o next
                        result.Add(ShowCardList(ref struCFG));
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_NEED_WAIT:
                        Thread.Sleep(200);
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_FAILED:
                        CHCNetSDK.NET_DVR_StopRemoteConfig(AcsEventHandle);
                        Debug.WriteLine("NET_SDK_GET_NEXT_STATUS_FAILED" + CHCNetSDK.NET_DVR_GetLastError().ToString(), "Error");
                        Flag = false;
                        break;
                    case CHCNetSDK.NET_SDK_GET_NEXT_STATUS_FINISH:
                        CHCNetSDK.NET_DVR_StopRemoteConfig(AcsEventHandle);
                        Flag = false;
                        break;
                    default:
                        Debug.WriteLine("NET_SDK_GET_NEXT_STATUS_UNKOWN" + CHCNetSDK.NET_DVR_GetLastError().ToString(), "Error");
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
            //string Major = ProcessMajorType(ref struEventCfg.dwMajor);
            //Cards.majorType = Major;
            //ProcessMinorType(ref struEventCfg);
            //Cards.minorType = CsTemp;
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
