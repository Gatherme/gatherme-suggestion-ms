using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using gatherme_suggestion_ms.Models;
using gatherme_suggestion_ms.Service;
using gatherme_suggestion_ms.Settings;
namespace gatherme_suggestion_ms.Controllers
{
    [ApiController]
    public class CategoryController
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
        public async Task NewCategory(Category category)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                CategoryService myService = new CategoryService(client);
                myService.addCategory(category);
                await myService.CreateCategory(myService.Categories);
            }
        }
        [HttpGet("[controller]/[action]")]
        public async Task<List<User>> FilterByCategory(string name)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                Category category = new Category {
                    Name = name
                };
                CategoryService myService = new CategoryService(client);
                myService.addCategory(category);
                return await myService.GetUsers(myService.Categories);
            }
        }
    }
}