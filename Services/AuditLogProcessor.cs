using System.Text.Json;
using Azure.Messaging.ServiceBus;
using VidizmoBackend.Data;
using VidizmoBackend.Models;

namespace VidizmoBackend.Services
{
    public class AuditLogProcessor : BackgroundService {
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _svc;

        public AuditLogProcessor(ServiceBusClient client, IConfiguration config, IServiceProvider svc) {
            _processor = client.CreateProcessor(config["AzureServiceBus:QueueName"], new ServiceBusProcessorOptions());
            _svc = svc;
        }

        protected override async Task ExecuteAsync(CancellationToken token) {
            _processor.ProcessMessageAsync += async args => {
            var body = args.Message.Body.ToString();
            var log = JsonSerializer.Deserialize<AuditLog>(body);
            using var scope = _svc.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.AuditLogs.Add(log!);
            await db.SaveChangesAsync(token);
            await args.CompleteMessageAsync(args.Message, token);
            };

            _processor.ProcessErrorAsync += args => {
            Console.Error.WriteLine($"Error receiving message: {args.Exception}");
            return Task.CompletedTask;
            };

            await _processor.StartProcessingAsync(token);
        }

        public override async Task StopAsync(CancellationToken token) {
            await _processor.StopProcessingAsync(token);
            await base.StopAsync(token);
        }
    }

}