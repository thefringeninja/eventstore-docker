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
            // var loopback = IPAddress.Loopback;

            var settings = new EventStoreClientSettings {
                ConnectivitySettings = {
                    // DnsGossipSeeds = new[] {
                    //     new DnsEndPoint("node1.eventstoredb.local", 2113),
                    //     new DnsEndPoint("node2.eventstoredb.local", 2114),
                    //     new DnsEndPoint("node3.eventstoredb.local", 2115),
                    // }
                    DnsGossipSeeds = new [] {
                        new DnsEndPoint("node1.eventstore", 1114), 
                        new DnsEndPoint("node2.eventstore", 2114), 
                        new DnsEndPoint("node3.eventstore", 3114), 
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
