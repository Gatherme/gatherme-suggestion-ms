using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using gatherme_suggestion_ms.Models;
using gatherme_suggestion_ms.Service;
using gatherme_suggestion_ms.Settings;
namespace gatherme_suggestion_ms.Controllers
{
    [ApiController]
    public class SuggestionController
    {
        [Route("[controller]")]
        [Route("[controller]/[action]")]
        public async Task<List<Suggestion>> Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                SuggestionService myService = new SuggestionService(client);
                return await myService.getAllSuggestions();
            }
        }

        [HttpPost("[controller]/[action]")]
        public async Task<IList<Suggestion>> CreateSuggest(User user)
        {

            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                SuggestionService myService = new SuggestionService(client);
                UserService myService2 = new UserService(client);
                myService2.addUser(user);
                IList<Suggestion> toReturn = await myService.CreateSuggestedRelation(myService2.Users);
                UserInfo metadata = new UserInfo
                {
                    User = user,
                    Suggestions = toReturn
                };
                myService2.addMetadata(metadata);
                await myService2.CreateRelationSuggUser(myService2.UserInfos);
                return toReturn;
            }
        }

        [HttpPost("[controller]/[action]")]
        public async Task Deactivate(Suggestion suggestion)
        {
            var settings = ConnectionSettings.CreateBasicAuth("bolt://localhost:7687", "neo4j", "admin");
            using (var client = new Neo4JClient(settings))
            {
                SuggestionService myService = new SuggestionService(client);
                myService.addSuggestion(suggestion);
                await myService.ChangeIsActive(myService.Suggestions);
            }
        }
    }
}