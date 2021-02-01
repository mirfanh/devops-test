using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;
using System.Text.Json;

namespace FileProcessor
{
    public class PayLoad
    {
        [JsonPropertyName("products")]
        public IList<Product> Products { get; set; }
        [JsonPropertyName("transmissionsummary")]
        public TransmissionSummary TransmissionSummary { get; set; }
        [JsonIgnore]
        public bool IsInvalidRecordCount => TransmissionSummary.RecordCount != Products.Count;
        [JsonIgnore]
        public bool IsInvalidQuantitySum => TransmissionSummary.QuantitySum != Products.Sum(x => x.Quantity);
        public static PayLoad Deserialize(string payLoad) => JsonSerializer.Deserialize<PayLoad>(payLoad);
    }
}
