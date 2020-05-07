using Neo4j.Driver;
using gatherme_suggestion_ms.Settings;
using System;
using System.Threading.Tasks;

namespace gatherme_suggestion_ms
{
    public class Neo4JClient : IDisposable
    {
        //public static string uri = "bolt://localhost:7687";
        public static string uri = "bolt://172.17.0.1:7687";
        public static string user = "neo4j";
        public static string password="admin";
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
                "CREATE CONSTRAINT ON (u:User) ASSERT u.name IS UNIQUE",
                "CREATE CONSTRAINT ON (u:User) ASSERT u.id IS UNIQUE",
                "CREATE CONSTRAINT ON (s:Suggestion) ASSERT s.id IS UNIQUE",
                "CREATE INDEX ON :Suggestion(isActive)",
                "CREATE CONSTRAINT ON (c:Category) ASSERT c.name IS UNIQUE",
                "CREATE CONSTRAINT ON (l:Like) ASSERT l.name IS UNIQUE",
                "CREATE CONSTRAINT ON (r:Report) ASSERT r.id IS UNIQUE",
                "CREATE(c:Category{name: \"Academico\"})",
                "CREATE(c:Category{name: \"Deporte\"})",
                "CREATE(c:Category{name: \"Juegos\"})",
                "CREATE(c:Category{name: \"Cultural\"})",
                "CREATE(c:Category{name: \"Comidas\"})",
                "CREATE(c:Category{name: \"Fiesta\"})",
                "CREATE(c:Category{name: \"Otros\"})"
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