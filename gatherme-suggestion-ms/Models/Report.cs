using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Models
{
    public class Report
    {
        [JsonProperty("id")]
        public string Id {get; set;}
        [JsonProperty("commentary")]
        public string Commentary {get;set;}
    }
}