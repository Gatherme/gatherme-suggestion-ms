using gatherme_suggestion_ms.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using gatherme_suggestion_ms.Serializer;
using System.Collections;
namespace gatherme_suggestion_ms.Service
{
    public class LikeService : ILikeService
    {
        private Neo4JClient client;
        private ArrayList myLikes;
        private ArrayList myLikeInfos;
        /*Contructor*/
        public LikeService(Neo4JClient client)
        {
            this.client = client;
            this.myLikes = new ArrayList();
            this.myLikeInfos = new ArrayList();
        }
        
        /*DB operations*/
        public async Task CreateLike(IList<Like> likes)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $likes AS like")
            .AppendLine("CREATE(:Like{name: like.name})")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            try
            {
                await session.RunAsync(cypher, new Dictionary<string, object>() { { "likes", ParameterSerializer.ToDictionary(likes) } });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task CreateRelationshipLike(IList<LikeInfo> likeMetadata)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $likeMetadata AS likeMetadata")
            //find like
            .AppendLine("MATCH (l:Like { name: likeMetadata.like.name })")
            //category
            .AppendLine("WITH likeMetadata, l")
            .AppendLine("UNWIND likeMetadata.category AS category")
            .AppendLine("MATCH (c:Category { name: category.name})")
            .AppendLine("MERGE (l)-[r:HAVE]->(c)")
            .AppendLine("RETURN l.name, type(r), c.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "likeMetadata", ParameterSerializer.ToDictionary(likeMetadata) } });
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        public async Task<List<Like>> getAllLikes()
        {
            //MATCH (n:Like) RETURN n
            string cypher = new StringBuilder()
            .AppendLine("MATCH (n:Like)")
            .AppendLine("RETURN n.name")
            .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<Like> likesList = new List<Like>();
            try
            {
                var reader = await session.RunAsync(cypher);
                while (await reader.FetchAsync())
                {
                    // Each current read in buffer can be reached via Current
                    foreach (var item in reader.Current.Values)
                    {
                        Like aux = new Like
                        {
                            Name = item.Value.ToString()
                        };
                        System.Console.WriteLine(item.Value);
                        likesList.Add(aux);
                    }

                }
            }
            finally
            {
                await session.CloseAsync();
            }
            return likesList;
        }

        public async Task<List<User>> GetUsers(IList<Like> likes)
        {
            //MATCH (l:Like {name: "golf"})<-[:LIKE]-(u:User) RETURN u
            string cypher = new StringBuilder()
              .AppendLine("UNWIND $likes AS like")
              .AppendLine("MATCH (l:Like { name: like.name })")
              .AppendLine("MATCH (l)<-[:LIKE]-(u:User)")
              .AppendLine("RETURN u.id, u.name")
              .ToString();
            var session = client.GetDriver().AsyncSession(o => o.WithDatabase("neo4j"));
            List<User> users = new List<User>();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "likes", ParameterSerializer.ToDictionary(likes) } });
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
        public void addLike(Like like)
        {
            this.myLikes.Add(like);
        }
        public void addMetadata(LikeInfo likeInfo)
        {
            this.myLikeInfos.Add(likeInfo);
        }



        public IList<Like> likes
        {
            get
            {
                return myLikes.ToArray(typeof(Like)) as Like[];
            }
        }
        public IList<LikeInfo> likeInfos
        {
            get
            {
                return myLikeInfos.ToArray(typeof(LikeInfo)) as LikeInfo[];
            }
        }
    }
}