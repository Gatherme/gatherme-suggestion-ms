using Newtonsoft.Json;
using System.Collections.Generic;
namespace gatherme_suggestion_ms.Models
{
    public class UserInfo
    {
        [JsonProperty("user")]
        public User User {get;set;}
        [JsonProperty("likes")]
        public IList<Like> Likes {get;set;}
        [JsonProperty("gathers")]
        public IList<User> Gathers {get;set;}
        [JsonProperty("suggestion")]
        public  IList<Suggestion> Suggestions {get;set;}
    }
}