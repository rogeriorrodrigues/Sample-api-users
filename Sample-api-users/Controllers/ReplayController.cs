
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Sample.Controllers
{

    public class Model
    {

        public class DataModel
        {

            public string Operation { get; set; }
            public List<RevocationUser> RevocationUsers { get; set; }


        }

        public class RevocationUser
        {

            public string ActionDateTime { get; set; }
            public int Identification { get; set; }
            public string Name { get; set; }

        }

        public DataModel Data { get; set; }

    }


    [ApiController]
    [Route("[controller]")]
    public class ReplayController : Controller
    {

        private readonly ILogger<ReplayController> _logger;

        public ReplayController(ILogger<ReplayController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post(Model model)
        {

            return Ok(model);

        }
    }
}
