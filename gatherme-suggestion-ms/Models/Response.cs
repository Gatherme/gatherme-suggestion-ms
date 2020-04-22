using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Models
{
    public class Response
    {
        [JsonProperty("ans")]
        public string Ans{get;set;}
        [JsonProperty("error")]
        public string Error{get;set;}
    }
}