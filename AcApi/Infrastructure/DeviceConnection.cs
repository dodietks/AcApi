using AcApi.Infrastructure.Exception;
using AcApi.Models;
using AcApi.Utils;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AcApi.Infrastructure
{
    public class DeviceConnection
    {
        private readonly ILogger<DeviceConnection> _logger;
        private readonly ConfigurationOptions _configuration;
        private int _userId;

        public DeviceConnection(ConfigurationOptions configuration, ILogger<DeviceConnection> logger)
        {
            _logger = logger;
            _configuration = configuration;
            CHCNetSDK.NET_DVR_Init();
            CHCNetSDK.NET_DVR_SetLogToFile(3, _configuration.LogPath, true);
        }

        public int DoLogin(Gate gate)
        {
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            struLoginInfo.sDeviceAddress = gate.Ip;
            struLoginInfo.sUserName = gate.Name;
            struLoginInfo.sPassword = gate.Password;
            struLoginInfo.byLoginMode = 0;
            struLoginInfo.byVerifyMode = 0;
            ushort.TryParse(gate.Port, out struLoginInfo.wPort);

            _userId = -1;
            _userId = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
            if (_userId == -1)
            {
                throw new BusinessRuleExpcetion("Anthentication fail!");
            }
            else
            {
                uint nErr = CHCNetSDK.NET_DVR_GetLastError();
                if (nErr == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
                {
                    _logger.LogInformation("user name or password error!");
                    if (1 == struDeviceInfoV40.bySupportLock)
                    {
                        string strTemp1 = string.Format($"Left {struDeviceInfoV40.byRetryLoginTime} try opportunity");
                        _logger.LogInformation(strTemp1);
                    }
                }
                else if (nErr == CHCNetSDK.NET_DVR_USER_LOCKED)
                {
                    if (1 == struDeviceInfoV40.bySupportLock)
                    {
                        string strTemp1 = string.Format($"user is locked, the remaining lock time is {struDeviceInfoV40.dwSurplusLockTime}");
                    }
                }
            }
            return _userId;
        }
    }
}
