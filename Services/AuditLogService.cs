using System.Text.Json;
using Azure.Messaging.ServiceBus;
using VidizmoBackend.Models;

namespace VidizmoBackend.Services
{
    public class AuditLogService {
        private readonly ServiceBusSender _sender;

        public AuditLogService(ServiceBusClient client, IConfiguration config) {
            _sender = client.CreateSender(config["AzureServiceBus:QueueName"]);
        }

        public async Task SendLogAsync(AuditLog log) {
            var json = JsonSerializer.Serialize(log);
            await _sender.SendMessageAsync(new ServiceBusMessage(json));
        }
    }

}