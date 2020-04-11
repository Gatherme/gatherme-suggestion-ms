using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
namespace gatherme_suggestion_ms.Service
{
    public interface ISuggestionService
    {
         IList<Suggestion> Suggestions {get;}
         IList<SuggestionInfo> SuggestionInfos {get;}
    }
}