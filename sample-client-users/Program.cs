using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace sample_client_users
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var response = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "https://login.microsoftonline.com/seedazb2c.onmicrosoft.com/oauth2/v2.0/token",
                ClientId = "46c429ba-efed-480c-8688-e0e352054b19",
                ClientSecret = "-glj_FnB4yinq2~XN3LUefWQf05z7Xj-t6",
                Scope = "https://seedazb2c.onmicrosoft.com/services/.default"

                //Address = $"https://login.microsoftonline.com/779811d8-4753-4c34-baeb-6b53957d52e3/oauth2/v2.0/token",
                //ClientId = "c929b1c7-f31f-46ec-b7f9-42385edcb459",
                //ClientSecret = "0D4JQzXACcjI9.24t-S9d~8yT-i5_xFLQ0",
                //Scope = "api://7a3837da-9171-4677-9fcd-e66362b054c8/.default"

            }).Result;

            var accessToken = response.AccessToken;


            using (HttpClient httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44323/SecretData");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage responseApi = httpClient.SendAsync(request).Result;
                responseApi.EnsureSuccessStatusCode();
                string responseBody = responseApi.Content.ReadAsStringAsync().Result;

                Console.WriteLine("Token:");
                Console.WriteLine(accessToken);

                Console.WriteLine("Response:");
                Console.WriteLine(responseBody);
            }


            Console.Read();
        }
    }
}
