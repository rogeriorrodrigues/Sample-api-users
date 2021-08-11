using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Polly;
using System;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {
        static string connectionString = "Endpoint=sb://seed-bus-p.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=aCGocpyZ2UYily2uP9SRZP0QV2rgVp52WJB2OaT/2Wg=";
        static string topicName = "topicSampleSeed";
        static string queueName = "queueSampleSeed";
        static string subscriptionName = "consumer1";

        static ServiceBusClient client;
        static ServiceBusProcessor processor;
        static int errors = 0;
        static async Task Main()
        {


            var retryPolicyNeedsTrueResponse =
               Policy.HandleResult<bool>(b => b != true)
               .WaitAndRetry(15, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2));

            bool result = retryPolicyNeedsTrueResponse.Execute(() => ReciveMessage());


            Console.ReadKey();

            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
            await client.DisposeAsync();


        }

        private static bool ReciveMessage()
        {
            try
            {
                connectionString = GetConnectionStringEnvironment().Result;
                Console.WriteLine($"{connectionString}");
                ReciveMessageQueue().Wait();
                ReciveMessageTopic().Wait();
                    
                return true;
            }
            catch (Exception ex)
            {
                errors++;
                Console.WriteLine($"Erro {errors} retry {ex.Message}");

                return false;
            }

        }

        

        

        private static async Task ReciveMessageQueue()
        {
            client = new ServiceBusClient(connectionString);
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            processor.ProcessMessageAsync += MessageHandlerQueue;
            processor.ProcessErrorAsync += ErrorHandler;
            await processor.StartProcessingAsync();
        }

        private static async Task ReciveMessageTopic()
        {
            client = new ServiceBusClient(connectionString);
            processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());
            processor.ProcessMessageAsync += MessageHandlerTopic;
            processor.ProcessErrorAsync += ErrorHandler;
            await processor.StartProcessingAsync();
        }

        static async Task MessageHandlerTopic(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body} from subscription: {subscriptionName}");
            await args.CompleteMessageAsync(args.Message);
        }

        static async Task MessageHandlerQueue(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body} from queue: {queueName}");
            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Erro ErrorHandler: {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private static async Task<string> GetConnectionStringEnvironment()
        {
            var keyvaultUrl = "https://seedkeys.vault.azure.net/";
            var client = new SecretClient(
                vaultUri: new Uri(keyvaultUrl),
                credential: new EnvironmentCredential()
            );
            var key1 = await client.GetSecretAsync("ServiceBusCns");
            return key1.Value.Value;
        }

        private static async Task<string> GetConnectionString()
        {

            var client2 = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(
            async (string auth, string resp, string scope) =>
            {

                var authCtx = new AuthenticationContext(auth);
                var credencial = new ClientCredential("69ee09e4-c51b-4f79-b9ca-39f56ccd5545", "H.g5syVaC9Mxl.8Wb-9AN7Ppno6I~ZE~zJ");
                var result = await authCtx.AcquireTokenAsync(resp, credencial);
                if (result == null)
                    throw new InvalidCastException("Erro");
                return result.AccessToken;

            }));

            var key2 = await client2.GetSecretAsync("https://seedkeys.vault.azure.net/", "ServiceBusCns");
            return key2.Value;
        }
    }
}
