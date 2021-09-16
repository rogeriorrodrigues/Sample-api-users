using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ConsoleClientAD
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
            var response = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"https://login.microsoftonline.com/779811d8-4753-4c34-baeb-6b53957d52e3/oauth2/v2.0/token",
                ClientId = "9d2f260c-829b-4101-91da-1440129d3601",
                ClientSecret = "KXW49.wcgogWSC3-NJ05.41xzy9PJwD_P1",
                Scope = "api://47b1d030-f6ff-41a8-8933-c52235a2651b/.default"

            }).Result;

            var accessToken = response.AccessToken;


            using (HttpClient httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44316/api/WeatherForecast");
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
