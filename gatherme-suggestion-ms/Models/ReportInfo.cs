using Newtonsoft.Json;
namespace gatherme_suggestion_ms.Models
{
    public class ReportInfo
    {
        [JsonProperty("report")]
        public Report Report{get;set;}
        [JsonProperty("userReported")]
        public User UserReported{get;set;}
        [JsonProperty("user")]
        public User User{get;set;}

    }
}