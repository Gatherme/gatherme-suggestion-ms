using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Models
{
    public class Category
    {
        [JsonProperty("name")]
        public string Name{get;set;}
    }
}