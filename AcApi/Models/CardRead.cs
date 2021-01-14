using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcApi.Models
{
    public class CardRead
    {
        [JsonProperty("id")]
        [Required]
        public string id { get; set; }

        [JsonProperty("numeroCartao")]
        [Required]
        public string cardNumber { get; set; }

        [JsonProperty("tipoPrincipal")]
        [Required]
        public string majorType { get; set; }

        [JsonProperty("tipoSecundario")]
        [Required]
        public string minorType { get; set; }

        [JsonProperty("data")]
        [Required]
        public string dateTime { get; set; }
    }
}
