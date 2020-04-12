using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using gatherme_suggestion_ms.Models;
using gatherme_suggestion_ms.Service;
using gatherme_suggestion_ms.Settings;
using System.Threading.Tasks;
namespace gatherme_suggestion_ms.Controllers
{
    [ApiController]
    public class LikeController : Controller
    {
        [HttpGet("[controller]")]
        [HttpGet("[controller]/[action]")]
        public async Task<List<Like>> Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                LikeService myService = new LikeService(client);
                return await myService.getAllLikes();
            }
        }
        [HttpPost("[controller]/[action]")]
        public async Task NewLike(Like like)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                LikeService myService = new LikeService(client);
                myService.addLike(like);
                await myService.CreateLike(myService.likes);
            }

        }
        [HttpPost("[controller]/[action]")]
        public async Task NewHave(LikeInfo like)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                LikeService myService = new LikeService(client);
                myService.addMetadata(like);
                await myService.CreateRelationshipLike(myService.likeInfos);
            }
        }
        [HttpGet("[controller]/[action]")]
        public async Task<List<User>> FilterByLike(Like like)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                LikeService myService = new LikeService(client);
                myService.addLike(like);
                return await myService.GetUsers(myService.likes);
            }
        }

    }
}