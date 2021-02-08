using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace AcApi.Models
{
    public class SmartCardDTO
    {
        [JsonProperty("Id")]
        [Required]
        public string Id { get; set; }

        [JsonProperty("GateId")]
        [Required]
        public string GateId { get; set; }

        [JsonProperty("DateTime")]
        [Required]
        public string DateTime { get; set; }
    }
}
