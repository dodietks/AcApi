using AcApi.Models;
using AcApi.Utils;
using System;
using System.Diagnostics;

namespace AcApi.Infrastructure
{
    public class DeviceConnection
    {
        private LoginOptions _loginOptions;
        private int _userId;

        public DeviceConnection(LoginOptions loginOptions)
        {
            this._loginOptions = loginOptions;
            CHCNetSDK.NET_DVR_Init();
            CHCNetSDK.NET_DVR_SetLogToFile(3, loginOptions.LogPath, true);
        }


        public int DoLogin()
        {
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            struLoginInfo.sDeviceAddress = _loginOptions.Ip;
            struLoginInfo.sUserName = _loginOptions.Name;
            struLoginInfo.sPassword = _loginOptions.Password;
            struLoginInfo.byLoginMode = 0;
            struLoginInfo.byVerifyMode = 0;
            ushort.TryParse(_loginOptions.Port, out struLoginInfo.wPort);

            _userId = -1;
            _userId = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
            if(_userId == -1)
            {
                throw new Exception("Anthentication fail!");
            }else
             {
                 uint nErr = CHCNetSDK.NET_DVR_GetLastError();
                 if (nErr == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
                 {
                     Debug.WriteLine("user name or password error!");
                     if (1 == struDeviceInfoV40.bySupportLock)
                     {
                         string strTemp1 = string.Format($"Left {0} try opportunity", struDeviceInfoV40.byRetryLoginTime);
                         Debug.WriteLine(strTemp1);
                     }
                 }
                 else if (nErr == CHCNetSDK.NET_DVR_USER_LOCKED)
                 {
                     if (1 == struDeviceInfoV40.bySupportLock)
                     {
                         string strTemp1 = string.Format($"user is locked, the remaining lock time is {0}", struDeviceInfoV40.dwSurplusLockTime);
                     }
                 }
            else
            {
                Debug.WriteLine("net error or dvr is busy!");
            }
            }
            return _userId;
        }
    }
}
