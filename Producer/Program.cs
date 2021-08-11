using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Polly;
using System;
using System.Threading.Tasks;

namespace Producer
{
    class Program
    {
        static string connectionString;
        static string queueName = "queueSampleSeed";
        static string topicName = "topicSampleSeed";
        static int errors = 0;

        static ServiceBusClient client;
        static ServiceBusSender senderQueue;
        static ServiceBusSender senderTopic;
        static IConfigurationRoot configurationRoot;

        const int numOfMessages = 1;

        static async Task Main(string[] args)
        {

            using IHost host = CreateHostBuilder(args).Build();


            var retryPolicyNeedsTrueResponse =
                Policy.HandleResult<bool>(b => b != true)
                .WaitAndRetry(15, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2));

            bool result = retryPolicyNeedsTrueResponse.Execute(() => SendMessage());

            await senderQueue.DisposeAsync();
            await senderTopic.DisposeAsync();
            await client.DisposeAsync();


            await host.RunAsync();

        }
        static IHostBuilder CreateHostBuilder(string[] args) =>
                    Host.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration((hostingContext, configuration) =>
                        {
                            configuration.Sources.Clear();

                            IHostEnvironment env = hostingContext.HostingEnvironment;

                            configuration
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

                            configurationRoot = configuration.Build();

                        });



        private static bool SendMessage()
        {
            try
            {
                //ConfigClientSBCns().Wait();

                ConfigClientSRbac();

                SenderQueue().Wait();
                SenderTopic().Wait();

                return true;

            }
            catch (Exception ex)
            {
                errors++;
                Console.WriteLine($"Erro {errors} retry {ex.Message}");
                return false;
            }


        }


        private static async Task ConfigClientSBCns()
        {
            connectionString = await GetConnectionStringEnvironment();
            Console.WriteLine($"{connectionString}");
            client = new ServiceBusClient(connectionString);
        }

        private static void ConfigClientSRbac()
        {
            var tenantId = configurationRoot.GetSection("busApp_tenantId").Value;
            var clientId = configurationRoot.GetSection("busApp_clientId").Value;
            var clientSecret = configurationRoot.GetSection("busApp_clientSecret").Value;
            var endpoint = configurationRoot.GetSection("bus_endpoint").Value;

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            client = new ServiceBusClient(endpoint, credential);
        }

        private static async Task<string> GetConnectionStringEnvironment()
        {
            var keyvaultUrl = configurationRoot.GetSection("kv_url").Value;
            var client = new SecretClient(
                vaultUri: new Uri(keyvaultUrl),
                credential: new EnvironmentCredential()
            );
            var key1 = await client.GetSecretAsync("ServiceBusCns");
            return key1.Value.Value;
        }

        private static async Task<string> GetConnectionString()
        {

            var kv_clientId = configurationRoot.GetSection("kv_clientId").Value;
            var kv_secretId = configurationRoot.GetSection("kv_secretId").Value;
            var kv_url = configurationRoot.GetSection("kv_url").Value;


            var client2 = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
                async (string auth, string resp, string scope) =>
                {

                    var authCtx = new AuthenticationContext(auth);
                    var credencial = new ClientCredential(kv_clientId, kv_secretId);
                    var result = await authCtx.AcquireTokenAsync(resp, credencial);
                    if (result == null)
                        throw new InvalidCastException("Erro");
                    return result.AccessToken;

                }));

            var key2 = await client2.GetSecretAsync(kv_url, "ServiceBusCns");
            return key2.Value;
        }


       

        private static async Task<ServiceBusMessageBatch> SenderTopic()
        {
            senderTopic = client.CreateSender(topicName);
            ServiceBusMessageBatch messageBatch = await senderTopic.CreateMessageBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                    throw new Exception($"The message {i} is too large to fit in the batch.");
            }

            await senderTopic.SendMessagesAsync(messageBatch);
            Console.WriteLine($"A batch of {numOfMessages} messages has been published to the topic.");

            return messageBatch;
        }

        private static async Task SenderQueue()
        {
            senderQueue = client.CreateSender(queueName);
            var messageBatch = await senderQueue.CreateMessageBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
                    throw new Exception($"The message {i} is too large to fit in the batch.");

                Console.WriteLine($"mensagem {i} add");
            }


            await senderQueue.SendMessagesAsync(messageBatch);
            Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");

        }





    }
}
