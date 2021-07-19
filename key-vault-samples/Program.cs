using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using System;
using System.Threading.Tasks;

namespace key_vault_samples
{
    class Program
    {
        static void Main(string[] args)
        {
            var key = GetKey().Result;
        }

        static async Task<JsonWebKey> GetKey()
        {
            var keyvaultUrl = "https://companyA.vault.azure.net/";
            var client = new KeyClient(
                vaultUri: new Uri(keyvaultUrl), 
                credential: new DefaultAzureCredential()
            );
            KeyVaultKey key = await client.GetKeyAsync("encriptionKey");
            return key.Key;
        }
    }
}
