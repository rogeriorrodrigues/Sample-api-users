using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sample_api_users.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SecretDataController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {

            return Ok(new
            {
                msg = "My Secret"
            });
        }
    }
}
