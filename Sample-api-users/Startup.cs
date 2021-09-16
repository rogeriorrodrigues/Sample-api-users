using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using Sample_api_users.Filters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Sample_api_users
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //"ClientId": "7a3837da-9171-4677-9fcd-e66362b054c8",
            //"Authority": "https://login.microsoftonline.com/779811d8-4753-4c34-baeb-6b53957d52e3",
            //"ValidIssuer": "https://login.microsoftonline.com/779811d8-4753-4c34-baeb-6b53957d52e3/v2.0",
            //"Audience": "api://7a3837da-9171-4677-9fcd-e66362b054c8"

            services.AddDbContext<SeedDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddControllers();

            //var clientIdCC = "68e218a6-6350-4015-b0dd-ecc1dfbf0f78";
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.Audience = clientIdCC;
            //        options.Authority = "https://login.microsoftonline.com/seedazb2c.onmicrosoft.com";
            //        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //        {
            //            ValidAudience = clientIdCC,
            //            ValidIssuer = "https://login.microsoftonline.com/b5882312-aafa-467b-b071-96db714bd4f5/v2.0"
            //        };
            //    });

            services.AddMicrosoftIdentityWebApiAuthentication(Configuration, "AzureAd");
            //services.AddApplicationInsightsTelemetry();
            TelemetryConfiguration.Active.InstrumentationKey = Configuration.GetSection("ApplicationInsights")["InstrumentationKey"];


            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1",

                    new OpenApiInfo
                    {
                        Title = "Sample.Api",
                        Version = "v1",
                        Description = "Sample.Api",
                    });

                var caminhoAplicacao = PlatformServices.Default.Application.ApplicationBasePath;
                var nomeAplicacao = PlatformServices.Default.Application.ApplicationName;
                var caminhoXmlDoc = Path.Combine(caminhoAplicacao, $"{nomeAplicacao}.xml");
                c.IncludeXmlComments(caminhoXmlDoc);

                //c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                //{
                //    Type = SecuritySchemeType.OAuth2,
                //    Flows = new OpenApiOAuthFlows
                //    {

                //        ClientCredentials = new OpenApiOAuthFlow
                //        {
                //            TokenUrl = new Uri("https://login.microsoftonline.com/779811d8-4753-4c34-baeb-6b53957d52e3/oauth2/v2.0/token"),
                //            AuthorizationUrl = new Uri("https://login.microsoftonline.com/779811d8-4753-4c34-baeb-6b53957d52e3/oauth2/v2.0/authorize"),
                //            Scopes = new Dictionary<string, string>
                //            {
                //                { "api://7a3837da-9171-4677-9fcd-e66362b054c8/.default", "api://7a3837da-9171-4677-9fcd-e66362b054c8/.default" },
                //            }
                //        },


                //    }
                //});

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {

                    Description = "Please enter into field the word 'Bearer' following by space and JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                 {
                           new OpenApiSecurityScheme
                           {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                            },
                            new List<string>()
                    }
                });


                //c.OperationFilter<AuthorizeCheckOperationFilter>();


            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseDeveloperExceptionPage();
            //Cultue
            var supportedCultures = new[]
            {
                new CultureInfo("pt-BR"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(culture: "pt-BR", uiCulture: "pt-BR"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                //c.OAuthClientId("c929b1c7-f31f-46ec-b7f9-42385edcb459");
                //c.OAuthClientSecret("0D4JQzXACcjI9.24t-S9d~8yT-i5_xFLQ0");
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sample.Api");
                c.OAuthAppName("swagger Dashboard");

            });


        }
    }
}
