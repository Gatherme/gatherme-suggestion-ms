using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Models
{
    public class User
    {
        [JsonProperty("id")]
        public string Id{get; set;}
        [JsonProperty("name")]
        public string Name{get;set;}
    }
}