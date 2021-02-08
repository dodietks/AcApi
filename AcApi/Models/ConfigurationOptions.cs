
using System.Collections.Generic;

namespace AcApi.Models
{
    public class ConfigurationOptions
    {
        public const string Configurations = "Configurations";
       
        public List<Gate> Gates { get; set; }
        public string LogPath { get; set; }
        public string EndPointSCG { get; set; }
        public string Token { get; set; }
        public int TaskDelay { get; set; }
        public int PostTimeOut { get; set; }
    }
}
