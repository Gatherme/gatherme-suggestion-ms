using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using gatherme_suggestion_ms.Models;
using gatherme_suggestion_ms.Service;
using gatherme_suggestion_ms.Settings;
using System.Threading.Tasks;
namespace gatherme_suggestion_ms.Controllers
{
    [ApiController]
    public class FakeController
    {
        [Route("[controller]")]
        [Route("[controller]/[action]")]
        public async Task Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                await client.CreateIndices();
            }
        }
    }
}