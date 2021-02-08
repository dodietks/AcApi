using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcApi.Models
{
    public class Gate
    {
        public const string Gates = "Gates";

        public string GateId { get; set; }
        public string Ip { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Port { get; set; }
    }
}
