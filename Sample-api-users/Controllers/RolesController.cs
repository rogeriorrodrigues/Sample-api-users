
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace Sample.Controllers
{

    public class Role
    {

        public string Name { get; set; }
    }


    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {


        private readonly ILogger<RolesController> _logger;

        public RolesController(ILogger<RolesController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(OperationId ="Papeis",Description = "Lista Papeis", Summary = "Papeis")]
        public IEnumerable<Role> Get()
        {
            return new List<Role> { new Role { Name = "Adm" } };
        }
    }
}