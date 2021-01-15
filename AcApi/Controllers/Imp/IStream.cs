using AcApi.Utils;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace AcApi.Controllers.Imp
{
    public class IStream
    {
        private Int32 m_lRealHandle = -1;
        private readonly Int16 channel = 1;
        public uint lastError = 0;

        CHCNetSDK.REALDATACALLBACK RealData = null;

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        public object GetStream(int m_UserID)
        {
            if (m_UserID < 0)
            {
                Debug.WriteLine("Please login the device firstly");
                return null;
            }

            if (m_lRealHandle < 0)
            {
                IntPtr hwnd = GetConsoleWindow();

                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = hwnd;//Ô¤ÀÀ´°¿Ú
                lpPreviewInfo.lChannel = channel;//Ô¤teÀÀµÄÉè±¸Í¨µÀ
                lpPreviewInfo.dwStreamType = 0;//ÂëÁ÷ÀàÐÍ£º0-Ö÷ÂëÁ÷£¬1-×ÓÂëÁ÷£¬2-ÂëÁ÷3£¬3-ÂëÁ÷4£¬ÒÔ´ËÀàÍÆ
                lpPreviewInfo.dwLinkMode = 0;//Á¬½Ó·½Ê½£º0- TCP·½Ê½£¬1- UDP·½Ê½£¬2- ¶à²¥·½Ê½£¬3- RTP·½Ê½£¬4-RTP/RTSP£¬5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- ·Ç×èÈûÈ¡Á÷£¬1- ×èÈûÈ¡Á÷
                lpPreviewInfo.dwDisplayBufNum = 1; //²¥·Å¿â²¥·Å»º³åÇø×î´ó»º³åÖ¡Êý
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;

                IntPtr pUser = new IntPtr();//ÓÃ»§Êý¾Ý

                //´ò¿ªÔ¤ÀÀ Start live view 
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_UserID, ref lpPreviewInfo, null/*RealData*/, pUser);

                AutomationElement elementWindow = AutomationElement.FromHandle(hwnd);
                Debug.WriteLine("Found UI Automation client-side provider for:");

                Debug.WriteLine(elementWindow.Current.Name);
                Console.WriteLine();
                if (m_lRealHandle < 0)
                {
                    lastError = CHCNetSDK.NET_DVR_GetLastError();
                    Debug.WriteLine($"NET_DVR_RealPlay_V40 failed, error code=  {0}", lastError);
                    return null;
                }
            }
            return null;
        }
    }
}
