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
        public string ip { get; set; }

        [JsonProperty("nome")]
        [Required]
        public string name { get; set; }

        [JsonProperty("senha")]
        [Required]
        public string password { get; set; }

        [JsonProperty("porta")]
        [Required]
        public string port { get; set; }

        [JsonProperty("funcao")]
        [Required]
        public int function { get; set; }
    }
}
