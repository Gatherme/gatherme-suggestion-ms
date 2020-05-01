using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using gatherme_suggestion_ms.Serializer;
using System.Collections;
namespace gatherme_suggestion_ms.Service
{
    public class UserService : IUserService
    {
        private Neo4JClient client;
        private ArrayList myUsers;
        private ArrayList myUserInfos;

        /*Contructor*/
        public UserService(Neo4JClient client)
        {
            this.client = client;
            this.myUsers = new ArrayList();
            this.myUserInfos = new ArrayList();
        }

        /*DB operations*/

        public async Task<string> CreateUser(IList<User> users)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $users AS user")
            .AppendLine("CREATE(u:User{id: user.id, name: user.name})")
            .AppendLine("RETURN u.id")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            string ans = "";
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "users", ParameterSerializer.ToDictionary(users) } });
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        ans += item.Value.ToString();
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return ans;
        }

        public async Task<string> CreateRelationLikeUser(IList<UserInfo> userMetadata)
        {
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $userMetadata AS userMetadata")
            //Find User
            .AppendLine("MATCH (u:User { id: userMetadata.user.id })")
            // User likes
            .AppendLine("WITH userMetadata, u")
            .AppendLine("UNWIND userMetadata.likes AS like")
            .AppendLine("MATCH (l:Like { name: like.name})")
            .AppendLine("MERGE (u)-[r:LIKE]->(l)")
            .AppendLine("RETURN u.name, type(r), l.name")
            .ToString();
            string ans = "";
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "userMetadata", ParameterSerializer.ToDictionary(userMetadata) } });
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        ans += item.Value.ToString() + " ";
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return ans;
        }
        public async Task<string> CreateRelationGatherUser(IList<UserInfo> userMetadata)
        {
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $userMetadata AS userMetadata")
            //Find User
            .AppendLine("MATCH (u:User { id: userMetadata.user.id })")
            // User GATHER
            .AppendLine("WITH userMetadata, u")
            .AppendLine("UNWIND userMetadata.gathers AS gather")
            .AppendLine("MATCH (g:User { id: gather.id})")
            .AppendLine("MERGE (u)-[r:GATHER]-(g)")
            .AppendLine("RETURN u.name, type(r), g.name")
            .ToString();
            string ans = "";
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "userMetadata", ParameterSerializer.ToDictionary(userMetadata) } });
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        ans += item.Value.ToString() + " ";
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return ans;
        }
        public async Task CreateRelationSuggUser(IList<UserInfo> userMetadata)
        {
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $userMetadata AS userMetadata")
            //Find User
            .AppendLine("MATCH (u:User { id: userMetadata.user.id })")
            // User GET Suggestion
            .AppendLine("WITH userMetadata, u")
            .AppendLine("UNWIND userMetadata.suggestion AS suggestion")
            .AppendLine("MATCH (s:Suggestion { id: suggestion.id})")
            .AppendLine("MERGE (u)-[r:GET]->(s)")
            .AppendLine("RETURN u.name, type(r), s.id")
            .ToString();
            string ans = "";
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "userMetadata", ParameterSerializer.ToDictionary(userMetadata) } });
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        ans += item.Value.ToString() + " ";
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<User>> GetAllUsers()
        {
            //MATCH (n:Like) RETURN n
            string cypher = new StringBuilder()
            .AppendLine("MATCH (n:User)")
            .AppendLine("RETURN n.id, n.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<User> users = new List<User>();
            try
            {
                var reader = await session.RunAsync(cypher);
                while (await reader.FetchAsync())
                {
                    int count = 0;
                    string id = "";
                    // Each current read in buffer can be reached via Current
                    foreach (var item in reader.Current.Values)
                    {
                        if ((count % 2 != 0))
                        {
                            User aux = new User
                            {
                                Id = id,
                                Name = item.Value.ToString()
                            };
                            users.Add(aux);
                        }
                        else
                        {
                            id = item.Value.ToString();
                        }
                        System.Console.WriteLine(item.Value);
                        count++;
                    }

                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return users;
        }

        public async Task<List<Like>> UsersLikes(IList<User> users)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $users AS user")
            .AppendLine("MATCH (u:User{id: user.id})")
            .AppendLine("MATCH (u)-[:LIKE]->(l:Like)")
            .AppendLine("RETURN l.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<Like> myLikes = new List<Like>();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "users", ParameterSerializer.ToDictionary(users) } });
                System.Console.WriteLine("****user's likes:");
                while (await reader.FetchAsync())
                {
                    foreach (var item in reader.Current.Values)
                    {
                        Like auxLike = new Like
                        {
                            Name = item.Value.ToString()
                        };
                        System.Console.WriteLine(item.Value.ToString());
                        myLikes.Add(auxLike);
                    }
                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return myLikes;
        }


        /*Interfaz*/
        public void addUser(User user)
        {
            this.myUsers.Add(user);
        }
        public void addMetadata(UserInfo userInfo)
        {
            this.myUserInfos.Add(userInfo);
        }



        public IList<User> Users
        {
            get
            {
                return myUsers.ToArray(typeof(User)) as User[];
            }
        }
        public IList<UserInfo> UserInfos
        {
            get
            {
                return myUserInfos.ToArray(typeof(UserInfo)) as UserInfo[];
            }
        }

    }
}