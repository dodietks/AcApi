
using System.Collections.Generic;

namespace AcApi.Models
{
    public class ConfigurationOptions
    {
        public const string Configurations = "Configurations";
       
        public List<Gate> Gates { get; set; }
        public string LogPath { get; set; }
    }
}
