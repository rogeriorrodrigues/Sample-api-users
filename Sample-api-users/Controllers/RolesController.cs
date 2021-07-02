
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;

namespace Sample.Controllers
{
    /// <summary>
    /// Classe de Cargos
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Nome do Cargo
        /// </summary>
        public string Name { get; set; }
    }


    /// <summary>
    /// Recurso que  gerencia roles
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {


        private readonly ILogger<RolesController> _logger;

        public RolesController(ILogger<RolesController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Obtem os cargos
        /// </summary>
        /// <returns>Retorna uma lista de Cargos</returns>
        [HttpGet]
        [SwaggerOperation(OperationId ="Papeis",Description = "Lista Papeis", Summary = "Papeis")]
        public IEnumerable<Role> Get()
        {
            return new List<Role> { new Role { Name = "Adm" } };
        }
    }
}