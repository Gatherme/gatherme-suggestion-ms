using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> NewLike(Like like)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                LikeService myService = new LikeService(client);
                myService.addLike(like);
                string aux = await myService.CreateLike(myService.likes);
                Response ans = new Response()
                {
                    Ans = aux
                };
                return Created(Neo4JClient.uri, ans);
            }

        }
        [HttpPost("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> NewIs(LikeInfo likeInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                LikeService myService = new LikeService(client);
                myService.addMetadata(likeInfo);
                string aux = await myService.CreateRelationshipLike(myService.likeInfos);
                Response ans = new Response()
                {
                    Ans = aux
                };
                return Created(Neo4JClient.uri, ans);
            }
        }
        [HttpGet("[controller]/[action]")]
        public async Task<List<User>> FilterByLike(string name)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                Like like = new Like
                {
                    Name = name
                };
                LikeService myService = new LikeService(client);
                myService.addLike(like);
                return await myService.GetUsers(myService.likes);
            }
        }

        [HttpGet("[controller]/[action]")]
        public async Task<bool> ExistLike(string name)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                Like like = new Like
                {
                    Name = name
                };
                LikeService myService = new LikeService(client);
                myService.addLike(like);
                List<bool> ans = await myService.ExistLike(myService.likes);
                return ans[0];
            }
        }


    }
}