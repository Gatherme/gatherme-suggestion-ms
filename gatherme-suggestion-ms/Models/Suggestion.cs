using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Models
{
    public class Suggestion
    {
        [JsonProperty("id")]
        public string Id {get;set;}
        [JsonProperty("isActive")]
        public bool IsActive{get;set;}
    }
}