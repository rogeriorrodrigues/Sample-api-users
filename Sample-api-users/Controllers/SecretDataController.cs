using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sample_api_users.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
