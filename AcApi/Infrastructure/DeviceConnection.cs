using AcApi.Models;
using AcApi.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
            //this.DoLogin();
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
                throw new Exception("Falha na autenticação!");
            }
            return _userId;
        }


        public void StartRemoteConfig(int loggedUser, CHCNetSDK.NET_DVR_ACS_EVENT_COND struCond)
        {
            uint dwSize = struCond.dwSize;
            IntPtr ptrCond = Marshal.AllocHGlobal((int)dwSize);
            Marshal.StructureToPtr(struCond, ptrCond, false);
            int result = CHCNetSDK.NET_DVR_StartRemoteConfig(loggedUser, CHCNetSDK.NET_DVR_GET_ACS_EVENT, ptrCond, (int)dwSize, null, IntPtr.Zero);
            if (-1 == result)
            {
                Marshal.FreeHGlobal(ptrCond);
                Debug.WriteLine("NET_DVR_StartRemoteConfig FAIL, ERROR CODE" + CHCNetSDK.NET_DVR_GetLastError().ToString(), "Error");
                CHCNetSDK.NET_DVR_Logout(loggedUser);
                Debug.WriteLine($"Successful to logout user {0} ", loggedUser);
                //return new List<CardRead>();
            }
            //return result;
        }


    }
}
