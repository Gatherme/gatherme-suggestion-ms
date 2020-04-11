using Neo4j.Driver;
using gatherme_suggestion_ms.Models;
using gatherme_suggestion_ms.Serializer;
using gatherme_suggestion_ms.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace gatherme_suggestion_ms
{
    public class Neo4JClient : IDisposable
    {
        private readonly IDriver driver;
        //public static IAsyncSession session;
        public Neo4JClient(IConnectionSettings settings)
        {
            driver = GraphDatabase.Driver(settings.Uri, settings.AuthToken);
            //session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
        }
        public async Task CreateIndices()
        {
            string[] queries = {
                "CREATE INDEX ON :User(name)",
                "CREATE INDEX ON :User(id)",
                "CREATE INDEX ON :Suggestion(id)",
                "CREATE INDEX ON :Suggestion(isActive)",
                "CREATE INDEX ON :Category(name)",
                "CREATE INDEX ON :Like(name)",
            };
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            try
            {
                foreach (var query in queries)
                {
                    await session.RunAsync(query);
                }
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        public async Task CreateUser(IList<User> users)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $users AS user")
            .AppendLine("MERGE (u:User {name: user.name})")
            .AppendLine("SET u = user")
            .ToString();
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            await session.RunAsync(cypher, new Dictionary<string, object>() { { "users", ParameterSerializer.ToDictionary(users) } });
        }
        public async Task CreateLike(IList<Like> likes)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $likes AS like")
            .AppendLine("MERGE (l:Like {name: like.name})")
            .AppendLine("SET l = like")
            .ToString();
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            await session.RunAsync(cypher, new Dictionary<string, object>() { { "likes", ParameterSerializer.ToDictionary(likes) } });
        }
        public async Task CreateCategory(IList<Category> categories)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $categories AS category")
            .AppendLine("MERGE (c:Category {name: category.name})")
            .AppendLine("SET c = category")
            .ToString();
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            await session.RunAsync(cypher, new Dictionary<string, object>() { { "categories", ParameterSerializer.ToDictionary(categories) } });
        }
        public async Task CreateSuggestion(IList<Suggestion> suggestions)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $suggestions AS suggestion")
            .AppendLine("MERGE (s:Suggestion {id: suggestion.id})")
            .AppendLine("SET s = suggestion")
            .ToString();
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            await session.RunAsync(cypher, new Dictionary<string, object>() { { "suggestions", ParameterSerializer.ToDictionary(suggestions) } });
        }
        public async Task CreateRelationshipUser(IList<UserInfo> userMetadata)
        {
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $userMetadata AS userMetadata")
            //Find User
            .AppendLine("MATCH (u:User { id: userMetadata.user.id })")
            // User likes
            .AppendLine("WITH userMetadata, u")
            .AppendLine("UNWIND userMetadata.likes AS like")
            .AppendLine("MATCH (l:Like { name: like.name})")
            .AppendLine("MERGE (u)-[r:LIKE]->(l)")
            // User Reports
            .AppendLine("WITH userMetadata, u")
            .AppendLine("UNWIND userMetadata.userReports AS userReport")
            .AppendLine("MATCH (e:User { id: userReport.id})")
            .AppendLine("MERGE (e)-[r:REPORT]->(u)")
            // User GATHER
            .AppendLine("WITH userMetadata, u")
            .AppendLine("UNWIND userMetadata.gathers AS gather")
            .AppendLine("MATCH (g:User { id: gather.id})")
            .AppendLine("MERGE (u)-[r:GATHER]-(g)")
            // User GET Suggestion
            .AppendLine("WITH userMetadata, u")
            .AppendLine("UNWIND userMetadata.suggestion AS suggestion")
            .AppendLine("MATCH (s:Suggestion { id: suggestion.id})")
            .AppendLine("MERGE (u)-[r:GET]->(s)")
            .ToString();
            try
            {
                var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "userMetadata", ParameterSerializer.ToDictionary(userMetadata) } });
                while (await reader.FetchAsync())
                {
                    // Each current read in buffer can be reached via Current
                    foreach (var item in reader.Current.Values)
                    {
                        System.Console.WriteLine(item);
                    }

                }
            }
            finally
            {
                await session.CloseAsync();
            }
        }
        /*TODO CreateRelationshipLike*/
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
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            var reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "likeMetadata", ParameterSerializer.ToDictionary(likeMetadata) } });
            while (await reader.FetchAsync())
            {
                // Each current read in buffer can be reached via Current
                foreach (var item in reader.Current.Values)
                {
                    System.Console.WriteLine(item);
                }

            }
        }
        /*TODO CreateRelationshipSuggestion*/
        public async Task CreateRelationshipSuggestion(IList<SuggestionInfo> suggestionMetadata)
        {
            string cypher = new StringBuilder()
            .AppendLine("UNWIND $suggestionMetadata AS suggestionMetadata")
            // find Suggestion
            .AppendLine("MATCH (s:Suggestion { id: suggestionMetadata.suggestion.id})")
            // suggested user
            .AppendLine("WITH suggestionMetadata, s")
            .AppendLine("UNWIND suggestionMetadata.suggestedUser AS suggestedUser")
            .AppendLine("MATCH (u:User { id: suggestedUser.id})")
            .AppendLine("MERGE (s)-[r:SUGGEST]->(u)")
            .AppendLine("RETURN s.id, type(r), u.name")
            .ToString();
            var session = driver.AsyncSession(o => o.WithDatabase("neo4j"));
            IResultCursor reader = await session.RunAsync(cypher, new Dictionary<string, object>() { { "suggestionMetadata", ParameterSerializer.ToDictionary(suggestionMetadata) } });
            while (await reader.FetchAsync())
            {
                // Each current read in buffer can be reached via Current
                foreach (var item in reader.Current.Values)
                {
                    System.Console.WriteLine(item);
                }

            }
        }
        public IDriver GetDriver() => this.driver;
        public void Dispose()
        {
            driver?.Dispose();
        }

    }
}