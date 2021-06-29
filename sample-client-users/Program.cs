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
                ClientSecret = "39g8.d05Gvp.hj-9K_D~-NpYmAbVN84Abs",
                Scope = "https://seedazb2c.onmicrosoft.com/services/.default"

            }).Result;
            var accessToken = response.AccessToken;

            var teste = String.Concat("", "");


            using (HttpClient httpClient = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:44323/SecretData");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage responseApi = httpClient.SendAsync(request).Result;
                responseApi.EnsureSuccessStatusCode();
                string responseBody = responseApi.Content.ReadAsStringAsync().Result;
                Console.WriteLine(responseBody);
            }
            Console.Read();
        }
    }
}
