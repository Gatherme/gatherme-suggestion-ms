using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
namespace gatherme_suggestion_ms.Service
{
    public interface IFakeData
    {
        IList<User> Users { get; }
        IList<Like> Likes { get; }
        IList<Suggestion> Suggestions { get; }
        IList<Category> Categories {get;}
        IList<UserInfo> UserInfos {get;}
        IList<SuggestionInfo> SuggestionInfos {get;}
        IList<LikeInfo> LikeInfos {get;}
    }
}