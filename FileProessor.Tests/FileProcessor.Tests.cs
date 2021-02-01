using System;
using Xunit;
using FileProcessor;
using System.Text.Json;

namespace FileProessor.Tests
{
    public class FileProcessorTests
    {
        [Fact]
        public void InvalidJson()
        {
            Assert.Throws<JsonException>(() => PayLoad.Deserialize(""));
            Assert.Throws<JsonException>(() => PayLoad.Deserialize("123"));
            Assert.Throws<JsonException>(() => PayLoad.Deserialize("{123"));
        }
        [Fact]
        public void RecordCountMismatch()
        {
            var json = "{\"products\":[],\"transmissionsummary\":{\"recordcount\":6}}";
            var p = PayLoad.Deserialize(json);
            Assert.True(p.IsInvalidRecordCount);
        }
        [Fact]
        public void QuantitySumMismatch()
        {
            var json = "{\"products\":[{\"qty\":10}],\"transmissionsummary\":{\"recordcount\":1,\"qtysum\":20}}";
            var p = PayLoad.Deserialize(json);
            Assert.True(p.IsInvalidQuantitySum);
        }
        [Fact]
        public void ValidJson()
        {
            var json = "{\"products\":[{\"qty\":10}],\"transmissionsummary\":{\"recordcount\":1,\"qtysum\":10}}";
            var p = PayLoad.Deserialize(json);
            Assert.False(p.IsInvalidRecordCount || p.IsInvalidQuantitySum);
        }
    }
}
