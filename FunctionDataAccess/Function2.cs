using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace FunctionDataAccess
{
    public static class Function2
    {
        [FunctionName("Function2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log, ExecutionContext context)
        {


            var settingsA = GetEnvironmentVariable("AzureWebJobsStorage");
            var settingsB = GetEnvironmentVariable("seedC");

            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var settingsC = config.GetConnectionString("default");

            if (settingsA != null)
                return new OkObjectResult($"{settingsA};{settingsB};{settingsC}");

            return new OkObjectResult("vazio");
        }

        public static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
