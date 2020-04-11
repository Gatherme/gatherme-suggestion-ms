using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
namespace gatherme_suggestion_ms.Service
{
    public interface ILikeService
    {
         IList<Like> likes {get;}
         IList<LikeInfo> likeInfos {get;}
    }
}