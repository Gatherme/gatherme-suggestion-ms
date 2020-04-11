using Microsoft.AspNetCore.Mvc;
namespace gatherme_suggestion_ms.Controllers
{
    [ApiController]
    public class HomeController
    {
        [HttpGet("[controller]")]
        [HttpGet("[controller]/[action]")]
        public  string Index()
        {
            return "welcome stranger";
        }
    }
}