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
    public class SuggestionController : Controller
    {
        [Route("[controller]")]
        [Route("[controller]/[action]")]
        public async Task<List<Suggestion>> Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                SuggestionService myService = new SuggestionService(client);
                return await myService.getAllSuggestions();
            }
        }
        [HttpGet("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSuggestion(string id)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                User user = new User {
                    Id = id
                };
                UserService aux = new UserService(client);
                aux.addUser(user);
                SuggestionService myService = new SuggestionService(client);
                List<SuggestionInfo> ans = await myService.getSuggestion(aux.Users);
                return Ok(ans);
            }         
        }

        [HttpPost("[controller]/[action]")]
        public async Task<IList<SuggestionInfo>> CreateSuggest(User user)
        {

            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                SuggestionService myService = new SuggestionService(client);
                UserService myService2 = new UserService(client);
                myService2.addUser(user);
                IList<SuggestionInfo> toReturn = await myService.CreateSuggestedRelation(myService2.Users);
                UserInfo metadata = new UserInfo
                {
                    User = user,
                    Suggestions = myService.Suggestions
                };
                myService2.addMetadata(metadata);
                await myService2.CreateRelationSuggUser(myService2.UserInfos);
                return toReturn;
            }
        }

        [HttpPut("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Deactivate(Suggestion suggestion)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                SuggestionService myService = new SuggestionService(client);
                myService.addSuggestion(suggestion);
                string aux = await myService.ChangeIsActive(myService.Suggestions);
                Response ans = new Response()
                {
                    Ans = aux
                };
                return Ok(ans);
            }
        }

    }
}