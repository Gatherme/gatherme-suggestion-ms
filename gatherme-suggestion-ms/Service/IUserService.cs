using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
namespace gatherme_suggestion_ms.Service
{
    public interface IUserService
    {
         IList<User> Users {get;}
         IList<UserInfo> UserInfos {get;}
    }
}