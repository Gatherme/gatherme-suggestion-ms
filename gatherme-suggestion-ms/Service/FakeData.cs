using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
namespace gatherme_suggestion_ms.Service
{
    public class FakeData : IFakeData
    {
        private static User user1 = new User
        {
            Id = "1",
            Name = "Bill"
        };
        private static User user2 = new User
        {
            Id = "2",
            Name = "Kill"
        };
        private static User user3 = new User
        {
            Id = "3",
            Name = "Novia"
        };
        private static Category category = new Category
        {
            Name = "Sport"
        };

        private static Like like = new Like
        {
            Name = "golf"
        };
        private static Like like2 = new Like
        {
            Name = "baseball"
        };
        private static Suggestion suggestion = new Suggestion
        {
            Id = "1",
            IsActive = true
        };
        private static LikeInfo metaDataLike = new LikeInfo
        {
            Category = category,
            Like = like
        };
        private static LikeInfo metaDataLike2 = new LikeInfo
        {
            Category = category,
            Like = like2
        };
        private static SuggestionInfo metaDataSuggestion = new SuggestionInfo
        {
            Suggestion = suggestion,
            SuggestedUser = user2
        };
        private static UserInfo metaDataUser = new UserInfo
        {
            Gathers = new[] { user3 },
            User = user1,
            Likes = new[] { like },
            UserReports = new[] { user2 },
            Suggestions = new[] { suggestion }
        };
        public IList<User> Users
        {
            get
            {
                return new[] { user1, user2, user3 };
            }
        }
        public IList<Like> Likes
        {
            get
            {
                return new[] { like, like2 };
            }
        }
        public IList<Suggestion> Suggestions
        {
            get
            {
                return new[] { suggestion };
            }
        }
        public IList<Category> Categories
        {
            get
            {
                return new[] { category };
            }
        }
        public IList<UserInfo> UserInfos
        {
            get
            {
                return new[] { metaDataUser };
            }
        }
        public IList<SuggestionInfo> SuggestionInfos
        {
            get
            {
                return new[] { metaDataSuggestion };
            }
        }
        public IList<LikeInfo> LikeInfos
        {
            get
            {
                return new[] { metaDataLike,metaDataLike2 };
            }
        }

    }
}