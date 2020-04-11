using Microsoft.AspNetCore.Mvc;
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
        [Route("[controller]/[action]")]
        public async Task<List<User>> Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                return await myService.GetAllUsers();
            }
        }
        [HttpPost("[controller]/[action]")]
        public async Task NewUser(User user)
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addUser(user);
                await myService.CreateUser(myService.Users);
            }
        }
        [HttpPost("[controller]/[action]")]
        public async Task NewReport(UserInfo userInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
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
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addMetadata(userInfo);
                await myService.CreateRelationGatherUser(myService.UserInfos);
            }
        }
        [HttpPost("[controller]/[action]")]
        public async Task NewGet(UserInfo userInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addMetadata(userInfo);
                await myService.CreateRelationGatherUser(myService.UserInfos);
            }
        }
        [HttpPost("[controller]/[action]")]
        public async Task NewLike(UserInfo userInfo)
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                UserService myService = new UserService(client);
                myService.addMetadata(userInfo);
                await myService.CreateRelationLikeUser(myService.UserInfos);
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