using Microsoft.AspNetCore.Mvc;

namespace WebApiCoreSeed.WebApi.Controllers
{
    [Route("")]
    //Ignored on swagger
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        /// <summary>
        /// redirects the root controller to swagger UI
        /// </summary>
        [HttpGet]
        public IActionResult Get()
        {
            return Redirect("swagger");
        }
    }
}