using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using gatherme_suggestion_ms.Models;
using gatherme_suggestion_ms.Service;
using gatherme_suggestion_ms.Settings;
using System.Net.Http;
using System.Net;
namespace gatherme_suggestion_ms.Controllers
{
    [ApiController]

    public class UserController : Controller
    {
        [Route("[controller]")]
        [Route("[controller]/[action]")]
        public async Task<List<User>> Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                return await myService.GetAllUsers();
            }
        }
        [HttpPost("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> NewUser(User user)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addUser(user);
                await myService.CreateUser(myService.Users);
            }
            return Created(Neo4JClient.uri, user);
        }
        [HttpPost("[controller]/[action]")]
        public async Task NewReport(UserInfo userInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addMetadata(userInfo);
                await myService.CreateRelationReportUser(myService.UserInfos);
            }
        }
        [HttpPost("[controller]/[action]")]
        public async Task NewGather(UserInfo userInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addMetadata(userInfo);
                await myService.CreateRelationGatherUser(myService.UserInfos);
            }
        }
        [HttpPost("[controller]/[action]")]
        [ProducesDefaultResponseType]
        public async Task<HttpResponseMessage> NewLike(UserInfo userInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addMetadata(userInfo);
                await myService.CreateRelationLikeUser(myService.UserInfos);
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }
        [HttpPost("[controller]/[action]")]
        public IList<UserInfo> Test()
        {
            FakeData ex = new FakeData();
            return ex.UserInfos;
        }
    }
}