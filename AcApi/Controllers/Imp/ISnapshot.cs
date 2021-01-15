using AcApi.Utils;
using System.Diagnostics;

namespace AcApi.Controllers.Imp
{
    public class ISnapshot
    {
        int channel = 1;
        public uint iLastErr = 0;
        string sJpegPicFileName;

        private string str;
        public object GetPicture(int m_UserID)
        {
        

        //Í¼Æ¬±£´æÂ·¾¶ºÍÎÄ¼þÃû the path and file name to save
        sJpegPicFileName = "C:/temp/JPEG_test.jpg";
        
        int lChannel = channel; //Í¨µÀºÅ Channel number
        
        CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
        lpJpegPara.wPicQuality = 0; //Í¼ÏñÖÊÁ¿ Image quality
        lpJpegPara.wPicSize = 0xff; //×¥Í¼·Ö±æÂÊ Picture size: 2- 4CIF£¬0xff- Auto(Ê¹ÓÃµ±Ç°ÂëÁ÷·Ö±æÂÊ)£¬×¥Í¼·Ö±æÂÊÐèÒªÉè±¸Ö§³Ö£¬¸ü¶àÈ¡ÖµÇë²Î¿¼SDKÎÄµµ
        
        //JPEG×¥Í¼ Capture a JPEG picture
        if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(m_UserID, lChannel, ref lpJpegPara, sJpegPicFileName))
        {
            iLastErr = CHCNetSDK.NET_DVR_GetLastError();
            str = "NET_DVR_CaptureJPEGPicture failed, error code= " + iLastErr;
            Debug.WriteLine(str);
            return null;
        }
        else
        {
            str = "Successful to capture the JPEG file and the saved file is " + sJpegPicFileName;
            Debug.WriteLine(str);
        }
        return null;
        }
    }
}
