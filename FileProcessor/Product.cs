using System;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace FileProcessor
{
    public class Product
    {
        [Key]
        [JsonPropertyName("sku")]
        public string SKU { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("category")]
        public string Category {
            get => category;
            set
            {
                category = value;
                var categories = value.Split(">").Select(x => x.Trim()).ToArray();
                CategoryL3 = categories[2];
            }
        }
        [JsonIgnore]
        public string CategoryL3 { get; private set; }
        private string category;
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        [JsonPropertyName("location")]
        public string Location { get; set; }
        [JsonPropertyName("qty")]
        public int Quantity { get; set; }
    }
}
