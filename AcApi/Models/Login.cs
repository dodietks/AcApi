using AcApi.Models.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcApi.Models
{
    public class Login
    {
        [JsonProperty("ip")]
        [Required]
        public string Ip { get; set; }

        [JsonProperty("nome")]
        [Required]
        public string Name { get; set; }

        [JsonProperty("senha")]
        [Required]
        public string Password { get; set; }

        [JsonProperty("porta")]
        [Required]
        public string Port { get; set; }

        [JsonProperty("funcao")]
        [Required]
        public FunctionEnum Function { get; set; }
    }
}
