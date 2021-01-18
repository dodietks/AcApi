using AcApi.Models.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcApi.Models
{
    public class LoginOptions
    {
        public const string ACS = "ACS";
        public string Ip { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
    }
}
