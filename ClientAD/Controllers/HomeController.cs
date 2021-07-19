using ClientAD.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientAD.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenAcquisition _tokenAcquisition;
        public HomeController(ILogger<HomeController> logger,IHttpClientFactory clientFactory, IConfiguration config, ITokenAcquisition tokenAcquisition)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _configuration = config;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var client = _clientFactory.CreateClient();

                var scope = _configuration["ApiWithRoles:ScopeForAccessToken"];
                var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });

                client.BaseAddress = new Uri(
                    _configuration["ApiWithRoles:ApiBaseAddress"]);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"api/WeatherForecast");

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var data = JArray.Parse(responseContent);

                }
                var errorList = new List<string> { $"Status code: {response.StatusCode}",  $"Error: {response.ReasonPhrase}" };
                
                var result  = JArray.FromObject(errorList);
            }
            catch (Exception e)
            {
                throw new ApplicationException($"Exception {e}");
            }


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
