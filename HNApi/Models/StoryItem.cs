using System.Text.Json.Serialization;
using HNApi.JsonConverters;

namespace HNApi.Models
{
    public class StoryItem
    {
        public int Id {  get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? By { get; set; }
        [JsonConverter(typeof(UnixToNullableDateTimeConverter))]
        public DateTime? Time { get; set; }
        public int Score { get; set; }
        public int Descendants { get; set; }
    }
}