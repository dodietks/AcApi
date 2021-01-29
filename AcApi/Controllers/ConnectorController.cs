﻿using AcApi.Controllers.Imp;
using AcApi.Infrastructure;
using AcApi.Models;
using AcApi.Models.Enum;
using AcApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AcApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConnectorController : ControllerBase
    {
        private readonly ILogger<ConnectorController> _logger;
        private readonly IAccessControl _accessControl;
        private readonly ISnapshot _snapshot;
        private readonly IStream _stream;
        private DeviceConnection _deviceConnection;

        public int m_UserID;

        public ConnectorController(ILogger<ConnectorController> logger, IAccessControl accessControl, ISnapshot snapshot, IStream stream, DeviceConnection deviceConnection)
        {
            _accessControl = accessControl;
            _snapshot = snapshot;
            _stream = stream;
            _deviceConnection = deviceConnection;
        }

        [HttpPost]
        public object Connector(Login Login)
        {
            //CHCNetSDK.NET_DVR_Init();
            //CHCNetSDK.NET_DVR_SetLogToFile(3, "./SdkLog/", true);
            //CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            //CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            //struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            //struLoginInfo.sDeviceAddress = Login.Ip;
            //struLoginInfo.sUserName = Login.Name;
            //struLoginInfo.sPassword = Login.Password;
            //struLoginInfo.byLoginMode = 0;
            //struLoginInfo.byVerifyMode = 0;
            //ushort.TryParse(Login.Port, out struLoginInfo.wPort);

            //int lUserID = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
            int _userID = _deviceConnection.DoLogin();
            Debug.WriteLine("Login efetuado, tentando recuperar eventos de acesso.");
            if (_userID >= 0 && Login.Function == FunctionEnum.AccessInfo)
            {
                Debug.WriteLine("Login efetuado, tentando recuperar eventos de acesso.");
                return _accessControl.GetACSEvent(_userID);
            }
            if (_userID >= 0 && Login.Function == FunctionEnum.Snapshot)
            {
                Debug.WriteLine("Login efetuado, tentanto capturar imagem.");
                return _snapshot.GetPicture(_userID);
            }
            if (_userID >= 0 && Login.Function == FunctionEnum.Stream)
            {
                Debug.WriteLine("Login efetuado, tentanto iniciar link.");
                return _stream.GetStream(_userID);
            }
            //else
            //{
            //    uint nErr = CHCNetSDK.NET_DVR_GetLastError();
            //    if (nErr == CHCNetSDK.NET_DVR_PASSWORD_ERROR)
            //    {
            //        Debug.WriteLine("user name or password error!");
            //        if (1 == struDeviceInfoV40.bySupportLock)
            //        {
            //            string strTemp1 = string.Format($"Left {0} try opportunity", struDeviceInfoV40.byRetryLoginTime);
            //            Debug.WriteLine(strTemp1);
            //        }
            //    }
            //    else if (nErr == CHCNetSDK.NET_DVR_USER_LOCKED)
            //    {
            //        if (1 == struDeviceInfoV40.bySupportLock)
            //        {
            //            string strTemp1 = string.Format($"user is locked, the remaining lock time is {0}", struDeviceInfoV40.dwSurplusLockTime);
            //        }
            //    }
                else
                {
                    Debug.WriteLine("net error or dvr is busy!");
                }
            //}
            return null;
        }
    }
}
