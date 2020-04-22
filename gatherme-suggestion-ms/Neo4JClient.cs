using Neo4j.Driver;
using gatherme_suggestion_ms.Settings;
using System;
using System.Threading.Tasks;

namespace gatherme_suggestion_ms
{
    public class Neo4JClient : IDisposable
    {
        //public static string uri = "bolt://localhost:7687";
        public static string uri = "bolt://172.22.0.1:7687";
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
                "CREATE CONSTRAINT ON (u:User) ASSERT u.id IS UNIQUE",
                "CREATE CONSTRAINT ON (s:Suggestion) ASSERT s.id IS UNIQUE",
                "CREATE INDEX ON :Suggestion(isActive)",
                "CREATE CONSTRAINT ON (c:Category) ASSERT c.name IS UNIQUE",
                "CREATE CONSTRAINT ON (l:Like) ASSERT l.name IS UNIQUE"
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
        
        public IDriver GetDriver() => this.driver;
        public void Dispose()
        {
            driver?.Dispose();
        }

    }
}