using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AcApi.Models
{
    public class CardRead
    {

        [JsonProperty("numeroCartao")]
        [Required]
        public string CardNumber { get; set; }

        [JsonProperty("tipoPrincipal")]
        [Required]
        public string MajorType { get; set; }

        [JsonProperty("tipoSecundario")]
        [Required]
        public string MinorType { get; set; }

        [JsonProperty("dateTimeInString")]
        [Required]
        public string DateTimeInString { get; set; }
    }
}
