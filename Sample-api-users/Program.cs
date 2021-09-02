using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;

namespace Sample_api_users
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            //.ConfigureLogging(logging =>
            //{
            //    logging.AddApplicationInsights(ai =>
            //    {
            //        ai.TrackExceptionsAsExceptionTelemetry = false;
            //    });
            //})
            .ConfigureLogging(
            builder =>
                {
                    builder.AddApplicationInsights();
                    builder.AddFilter<ApplicationInsightsLoggerProvider>("Geral", LogLevel.Information);
                }
            )
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
}
