using AcApi.Utils;
using System.Diagnostics;

namespace AcApi.Controllers.Imp
{
    public class ISnapshot
    {
        int channel = 1;
        public uint lastError = 0;
        string imagePath;

        public object GetPicture(int m_UserID)
        {
            //Í¼Æ¬±£´æÂ·¾¶ºÍÎÄ¼þÃû the path and file name to save
            imagePath = "C:/temp/JPEG_test.jpg";
        
        int lChannel = channel; //Í¨µÀºÅ Channel number
        
        CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
        lpJpegPara.wPicQuality = 0; //Í¼ÏñÖÊÁ¿ Image quality
        lpJpegPara.wPicSize = 0xff; //×¥Í¼·Ö±æÂÊ Picture size: 2- 4CIF£¬0xff- Auto(Ê¹ÓÃµ±Ç°ÂëÁ÷·Ö±æÂÊ)£¬×¥Í¼·Ö±æÂÊÐèÒªÉè±¸Ö§³Ö£¬¸ü¶àÈ¡ÖµÇë²Î¿¼SDKÎÄµµ
        
        //JPEG×¥Í¼ Capture a JPEG picture
        if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(m_UserID, lChannel, ref lpJpegPara, imagePath))
        {
            lastError = CHCNetSDK.NET_DVR_GetLastError();
            Debug.WriteLine($"NET_DVR_CaptureJPEGPicture failed, error code= {0}", lastError);
            CHCNetSDK.NET_DVR_Logout(m_UserID);
            Debug.WriteLine($"Successful to logout user {0} ", m_UserID);
            return null;
        }
        else
        {
            Debug.WriteLine($"Successful to capture the JPEG file and the saved file is {0} ", imagePath);
            CHCNetSDK.NET_DVR_Logout(m_UserID);
            Debug.WriteLine($"Successful to logout user {0} ", m_UserID);
        }
        return null;
        }
    }
}
