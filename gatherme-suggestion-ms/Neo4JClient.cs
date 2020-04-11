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
        
        public IDriver GetDriver() => this.driver;
        public void Dispose()
        {
            driver?.Dispose();
        }

    }
}