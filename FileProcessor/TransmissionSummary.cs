using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FileProcessor
{
    public class TransmissionSummary
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        [JsonPropertyName("recordcount")]
        public int RecordCount { get; set; }
        [JsonPropertyName("qtysum")]
        public int QuantitySum { get; set; }
    }
}
