using AcApi.Controllers.Imp;
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

        public int m_UserID = -1;

        public ConnectorController(ILogger<ConnectorController> logger, IAccessControl accessControl, ISnapshot snapshot)
        {
            _accessControl = accessControl;
            _snapshot = snapshot;
        }

        [HttpPost]
        public object Connector(Login Login)
        {
            CHCNetSDK.NET_DVR_Init();
            CHCNetSDK.NET_DVR_SetLogToFile(3, "./SdkLog/", true);
            CHCNetSDK.NET_DVR_USER_LOGIN_INFO struLoginInfo = new CHCNetSDK.NET_DVR_USER_LOGIN_INFO();
            CHCNetSDK.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new CHCNetSDK.NET_DVR_DEVICEINFO_V40();
            struDeviceInfoV40.struDeviceV30.sSerialNumber = new byte[CHCNetSDK.SERIALNO_LEN];

            struLoginInfo.sDeviceAddress = Login.Ip;
            struLoginInfo.sUserName = Login.Name;
            struLoginInfo.sPassword = Login.Password;
            struLoginInfo.byLoginMode = 0;
            struLoginInfo.byVerifyMode = 0;
            ushort.TryParse(Login.Port, out struLoginInfo.wPort);

            int lUserID = -1;
            lUserID = CHCNetSDK.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
            if (lUserID >= 0 && Login.Function == FunctionEnum.AccessInfo)
            {
                m_UserID = lUserID;
                Debug.WriteLine("Login efetuado, tentando recuperar eventos de acesso.");
                return _accessControl.GetACSEvent(m_UserID);
            }
            if (lUserID >= 0 && Login.Function == FunctionEnum.Snapshot)
            {
                m_UserID = lUserID;
                Debug.WriteLine("Login efetuado, tentanto capturar imagem.");
                return _snapshot.GetPicture(m_UserID);
            }
            else
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
            return null;
        }
    }
}
