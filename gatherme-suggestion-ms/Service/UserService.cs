using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using gatherme_suggestion_ms.Serializer;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        
        public async Task CreateUser(IList<User> users)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $users AS user")
            .AppendLine("CREATE(:User{id: user.id, name: user.name})")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            try
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "users", ParameterSerializer.ToDictionary(users) } });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateRelationLikeUser(IList<UserInfo> userMetadata)
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
            .ToString();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "userMetadata", ParameterSerializer.ToDictionary(userMetadata) } });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        public async Task CreateRelationReportUser(IList<UserInfo> userMetadata)
        {
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $userMetadata AS userMetadata")
            //Find User
            .AppendLine("MATCH (u:User { id: userMetadata.user.id })")
            // User Reports
            .AppendLine("WITH userMetadata, u")
            .AppendLine("UNWIND userMetadata.userReports AS userReport")
            .AppendLine("MATCH (e:User { id: userReport.id})")
            .AppendLine("MERGE (u)-[r:REPORT]->(e)")
            .ToString();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "userMetadata", ParameterSerializer.ToDictionary(userMetadata) } });
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        public async Task CreateRelationGatherUser(IList<UserInfo> userMetadata)
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
            .ToString();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "userMetadata", ParameterSerializer.ToDictionary(userMetadata) } });
            }
            finally
            {
                await session.CloseAsync();
            }
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
            .ToString();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "userMetadata", ParameterSerializer.ToDictionary(userMetadata) } });
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



        /*Interfaz*/
        public void addUser(User like)
        {
            this.myUsers.Add(like);
        }
        public void addMetadata(UserInfo likeInfo)
        {
            this.myUserInfos.Add(likeInfo);
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