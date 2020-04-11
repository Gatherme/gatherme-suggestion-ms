using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using gatherme_suggestion_ms.Settings;
using gatherme_suggestion_ms.Service;
namespace gatherme_suggestion_ms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            /* Test
            var service = new FakeData();
            RunAsync(service).GetAwaiter().GetResult();*/
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        public static async Task RunAsync(IFakeData service)
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {

                // Create Indices for faster Lookups:
                //await client.CreateIndices();

                //Create Base Data:
                await client.CreateUser(service.Users);
                await client.CreateCategory(service.Categories);
                await client.CreateLike(service.Likes);
                await client.CreateSuggestion(service.Suggestions);
                // Create Relationships:
                await client.CreateRelationshipLike(service.LikeInfos);
                await client.CreateRelationshipUser(service.UserInfos);
                await client.CreateRelationshipSuggestion(service.SuggestionInfos);
            }
        }
    }
}
