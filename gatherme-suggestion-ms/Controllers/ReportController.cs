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
    public class ReportController : Controller
    {
        [Route("[controller]")]
        [Route("[controller]/[action]")]
        public async Task<List<Report>> Index()
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                ReportService myService = new ReportService(client);
                return await myService.getAllReports();
            }
        }
        [HttpPost("[controller]")]
        [HttpPost("[controller]/[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Report(ReportInfo report)
        {
            var settings = ConnectionSettings.CreateBasicAuth(Neo4JClient.uri, Neo4JClient.user, Neo4JClient.password);
            using (var client = new Neo4JClient(settings))
            {
                ReportService myService = new ReportService(client);
                string ans = await myService.CeateReport(report);
                Response response = new Response {
                    Ans = ans
                };
                return Created(Neo4JClient.uri,response);
            }
        }
    }
}