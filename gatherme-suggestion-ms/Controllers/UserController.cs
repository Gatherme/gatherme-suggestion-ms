using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using gatherme_suggestion_ms.Models;
using gatherme_suggestion_ms.Service;
using gatherme_suggestion_ms.Settings;
namespace gatherme_suggestion_ms.Controllers
{
    [ApiController]

    public class UserController : Controller
    {
        [Route("[controller]")]
        [HttpGet("[controller]/[action]")]
        public async Task<List<User>> Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                return await myService.GetAllUsers();
            }
        }
        [HttpGet("[controller]/[action]")]
        public async Task<List<Like>> UsersLikes(string id)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                User auxUser = new User{
                    Id = id
                };
                myService.addUser(auxUser);
                List<Like> ans = await myService.UsersLikes(myService.Users);
                return ans;
            }
        }
        [HttpPost("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> NewUser(User user)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addUser(user);
                string aux = await myService.CreateUser(myService.Users);
                Response ans = new Response()
                {
                    Ans = aux
                };
                return Created(Neo4JClient.uri, ans);
            }
        }

        [HttpPost("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> NewGather(UserInfo userInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addMetadata(userInfo);
                string aux = await myService.CreateRelationGatherUser(myService.UserInfos);
                Response ans = new Response()
                {
                    Ans = aux
                };
                return Created(Neo4JClient.uri, ans);
            }
        }
        [HttpPost("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> NewLike(UserInfo userInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addMetadata(userInfo);
                string aux =  await myService.CreateRelationLikeUser(myService.UserInfos);
                Response ans = new Response()
                {
                    Ans = aux
                };
                return Created(Neo4JClient.uri, ans);
            }

        }
        [HttpPost("[controller]/[action]")]
        public IList<UserInfo> Test()
        {
            FakeData ex = new FakeData();
            return ex.UserInfos;
        }
    }
}