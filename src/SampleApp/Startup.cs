using System.Net;
using System.Net.Http;
using EventStore.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleApp {
    public class Startup {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            services.AddSingleton(ctx => ConfigureEventStoreDBClient(ctx.GetRequiredService<ILoggerFactory>()));
            services.AddHostedService<TestService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        static EventStoreClient ConfigureEventStoreDBClient(ILoggerFactory loggerFactory) {
            var settings = new EventStoreClientSettings {
                ConnectivitySettings = {
                    DnsGossipSeeds = new [] {
                        new DnsEndPoint("localhost", 2111), 
                        new DnsEndPoint("localhost", 2112), 
                        new DnsEndPoint("localhost", 2113)
                    }
                        
                },
                DefaultCredentials = new UserCredentials("admin", "changeit"),
                CreateHttpMessageHandler = () => new HttpClientHandler {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                },
                LoggerFactory = loggerFactory
            };

            return new EventStoreClient(settings);
        }
    }
}
