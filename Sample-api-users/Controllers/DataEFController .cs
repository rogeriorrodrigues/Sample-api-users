using Microsoft.AspNetCore.Mvc;
using Sample_api_users.Model;
using System.Data.SqlClient;
using System.Linq;


namespace Sample_api_users.Controllers
{
    /// <summary>
    /// Devolve informações do Dispositivo
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DataEFController : ControllerBase
    {

        private readonly SeedDbContext _ctx;
        public DataEFController(SeedDbContext ctx)
        {
            this._ctx = ctx;
        }

        [HttpGet]
        public IActionResult Index()
        {

            var result = _ctx.Set<SampleType>().ToList();
            return Ok(result);
        }

    }
}
