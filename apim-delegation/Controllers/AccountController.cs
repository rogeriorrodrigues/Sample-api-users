using apim_delegation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace apim_delegation.Controllers
{

   

    /// <summary>
    /// https://github.com/Azure/api-management-samples/blob/master/delegation/ContosoWebApplication/ContosoWebApplication/Controllers/AccountController.cs
    /// https://docs.microsoft.com/pt-br/rest/api/apimanagement/2021-01-01-preview/user/generatessourl
    /// https://docs.microsoft.com/en-us/rest/api/apimanagement/apimanagementrest/azure-api-management-rest-api-authentication
    /// https://docs.microsoft.com/en-us/azure/api-management/api-management-howto-setup-delegation
    /// https://msdn.microsoft.com/en-us/library/azure/dn776336.aspx#ListProducts
    /// </summary>
    /// <returns></returns>

    [Authorize]
    public class AccountController : Controller
    {

        private readonly IConfiguration _config;
        private readonly string _apimRestPK;
        private readonly string _apimRestId;
        private readonly DateTime _apimRestExpiry;
        public AccountController(IConfiguration config)
        {
            this._config = config;
            this._apimRestPK = this._config["ApimRestPK"];
            this._apimRestId = this._config["ApimRestId"];
            this._apimRestExpiry = DateTime.UtcNow.AddDays(10);
        }
        public async Task<IActionResult> Index()
        {

            var operations = Request.Query["operation"].ToString();
            if (operations == "SignOut")
                return Logout();

            return await GenerateTokenSsoUrl();
        }

       
        private async Task<IActionResult> GenerateTokenSsoUrl()
        {
            using (var client = new HttpClient())
            {
                var ApimRestHost = GetApimRestHostComplete();
                client.BaseAddress = new Uri(ApimRestHost);
                client.DefaultRequestHeaders.Add("Authorization", ApimRestAuthHeader());

                var userId = await GetUser();
                var apiVersionssourl = this._config["ApiVersionSSOUrl"];
                var response = await client.PostAsync("users/" + userId + "/generateSsoUrl?api-version=" + apiVersionssourl, this.GetContent(""));
                if (response.IsSuccessStatusCode)
                {
                    var receiveStream = response.Content;
                    var ssoUrlJson = await receiveStream.ReadAsStringAsync();

                    var ssoUrl = System.Text.Json.JsonSerializer.Deserialize<ResponseHttp>(ssoUrlJson);
                    return Redirect(ssoUrl.value.Replace(".portal.", ".developer."));
                }

                return View(new
                {
                    response = response,
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ApimRestHost = ApimRestHost,
                    ApimRestAuthHeader = ApimRestAuthHeader()
                });
            }
        }

        private async Task<string> GetUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var name = User.FindFirstValue("name");
            var email = User.FindFirstValue("emails");


            using (HttpClient httpClient = new HttpClient())
            {
                var apiversiongetusers = this._config["ApiVersionGetUsers"];
                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(ApimRestHostBasic(), "/users", "?api-version=", apiversiongetusers));

                httpClient.DefaultRequestHeaders.Add("Authorization", ApimRestAuthHeader());
                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var data = await response.Content.ReadAsStringAsync();
                var result = System.Text.Json.JsonSerializer.Deserialize<ModelUsers>(data);
                var apimuser = result.value.Where(_ => _.email == email).FirstOrDefault();
                if (apimuser == null)
                {
                    var userIdCreated = await CreateAuserInAPIM(userId, name, email);
                    return userIdCreated;
                }
                return apimuser.id.Replace("/users/", "");
            }

        }
        
        private async Task<string> CreateAuserInAPIM(string userId, string name, string email)
        {
            var apiversionputusers = this._config["ApiVersionpPutUsers"];
            using (var client = new HttpClient())
            {
                string ApimRestHost = GetApimRestHostComplete();
                client.BaseAddress = new Uri(ApimRestHost);
                client.DefaultRequestHeaders.Add("Authorization", ApimRestAuthHeader());

                var ApimUser = new
                {
                    firstName = name != null ? name.Split(" ").FirstOrDefault() : "-",
                    lastName = name != null ? name.Split(" ").LastOrDefault() : "-",
                    email = email,
                    password = Guid.NewGuid().ToString(),
                    state = "active"
                };

                var ApimUserJson = System.Text.Json.JsonSerializer.Serialize(ApimUser);
                var responseCreate = await client.PutAsync("users/" + userId + "?api-version=" + apiversionputusers, this.GetContent(ApimUserJson));
                if (!responseCreate.IsSuccessStatusCode)
                    throw new InvalidOperationException($"User create error {responseCreate.StatusCode}");

                return userId;
            }
        }

        private async Task<IActionResult> GetProducts()
        {

            var apiversiongetproduct = this._config["ApiVersionGetProduct"];
            using (HttpClient httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, string.Concat(ApimRestHostBasic(), "/products", "?api-version=", apiversiongetproduct));
                httpClient.DefaultRequestHeaders.Add("Authorization", ApimRestAuthHeader());

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody);
            }
        }

        public string ApimRestAuthHeader()
        {
            using (var encoder = new HMACSHA512(Encoding.UTF8.GetBytes(_apimRestPK)))
            {
                var dataToSign = this._apimRestId + "\n" + this._apimRestExpiry.ToString("O", CultureInfo.InvariantCulture);
                var hash = encoder.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                var signature = Convert.ToBase64String(hash);
                var encodedToken = string.Format("SharedAccessSignature uid={0}&ex={1:o}&sn={2}", this._apimRestId, this._apimRestExpiry, signature);
                return encodedToken;
            }
        }
        private string ApimRestHostBasic()
        {
            var serviceName = this._config["ServiceName"];
            var baseUrl = $"https://{serviceName}.management.azure-api.net";
            return baseUrl;
        }
        private string GetApimRestHostComplete()
        {

            var serviceName = this._config["ServiceName"];
            var subscriptionId = this._config["SubscriptionId"];
            var resourceGroup = this._config["ResourceGroup"];

            var ApimRestHost = $"https://{serviceName}.management.azure-api.net/subscriptions/{subscriptionId}/resourceGroups/{resourceGroup}/providers/Microsoft.ApiManagement/service/{serviceName}/";
            return ApimRestHost;
        }

        private StringContent GetContent(string content)
        {
            string requestBody = (string.IsNullOrEmpty(content)) ? "" : "{ \"properties\" :" + content + "}";
            return new StringContent(requestBody, Encoding.UTF8, "application/json");
        }

               
       

        public IActionResult Logout()
        {

            foreach (var cookie in Request.Cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
            var client_request_id = this._config["AzureAdB2C:ClientId"];
            var post_logout_redirect_uri = this._config["post_logout_redirect_uri"];
            var instance = this._config["AzureAdB2C:Instance"];
            var domain = this._config["AzureAdB2C:Domain"];
            var userFlow = this._config["AzureAdB2C:DomaSignUpSignInPolicyIdin"]; 
            var url = $"{instance}/{domain}/{userFlow}/oauth2/v2.0/logout?client-request-id={client_request_id}&post_logout_redirect_uri={post_logout_redirect_uri}";
            return Redirect(url);

        }
    }


}
