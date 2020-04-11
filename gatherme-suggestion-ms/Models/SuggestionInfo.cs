using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Models
{
    public class SuggestionInfo
    {
        [JsonProperty("suggestion")]
        public Suggestion Suggestion { get; set; }
        [JsonProperty("suggestedUser")]
        public User SuggestedUser { get; set; }
    }
}