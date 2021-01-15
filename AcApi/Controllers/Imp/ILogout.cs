using AcApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcApi.Controllers.Imp
{
    public class ILogout
    {
        public void Logout(int m_UserID)
        {
            CHCNetSDK.NET_DVR_Logout(m_UserID);
            m_UserID = -1;
        }
    }
}
