using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Models
{
    public class LikeInfo
    {
        [JsonProperty("like")]
        public Like Like {get;set;}
        [JsonProperty("category")]
        public Category Category {get;set;}
    }
}