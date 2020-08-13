using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SampleApp {
    public class TestService : IHostedService {
        readonly EventStoreClient     _client;
        readonly ILogger<TestService> _logger;

        public TestService(EventStoreClient client, ILogger<TestService> logger) {
            _client = client;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken) {
            var eventData = new EventData(
                Uuid.NewUuid(),
                "some-event",
                Encoding.UTF8.GetBytes("{\"id\": \"1\" \"value\": \"some value\"}")
            );

            _logger.LogInformation("Writing an event...");

            await _client.AppendToStreamAsync(
                "some-stream",
                StreamState.NoStream,
                new List<EventData> {
                    eventData
                },
                cancellationToken: cancellationToken
            );

            _logger.LogInformation("Done");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
