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
    public class CategoryController : Controller
    {
        [Route("[controller]")]
        [Route("[controller]/[action]")]
        public async Task<List<Category>> Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                CategoryService myService = new CategoryService(client);
                return await myService.GetAllCategories();
            }
        }
        [HttpPost("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> NewCategory(Category category)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                CategoryService myService = new CategoryService(client);
                myService.addCategory(category);
                string aux = await myService.CreateCategory(myService.Categories);
                Response ans = new Response()
                {
                    Ans = aux
                };
                return Created(Neo4JClient.uri, ans);
            }
        }
        [HttpGet("[controller]/[action]")]
        public async Task<List<User>> FilterByCategory(string name)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                Category category = new Category
                {
                    Name = name
                };
                CategoryService myService = new CategoryService(client);
                myService.addCategory(category);
                return await myService.GetUsers(myService.Categories);
            }
        }
        [HttpGet("[controller]/[action]")]
        public async Task<bool> ExistCategory(string name)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                Category category = new Category
                {
                    Name = name
                };
                CategoryService myService = new CategoryService(client);
                myService.addCategory(category);
                List<bool> ans = await myService.ExistCategory(myService.Categories);
                return ans[0];
            }
        }
        [HttpGet("[controller]/[action]")]
        public async Task<List<Like>> LikeByCategory(string name)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                Category category = new Category
                {
                    Name = name
                };
                CategoryService myService = new CategoryService(client);
                myService.addCategory(category);
                List<Like> ans = await myService.LikeByCategory(myService.Categories);
                return ans;
            }
        }
    }
}