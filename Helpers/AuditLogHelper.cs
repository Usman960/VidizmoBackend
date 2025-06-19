using System.Text.Json;

namespace VidizmoBackend.Helpers
{
    public static class AuditLogHelper
    {
        public static string BuildPayload(object? routeData = null, object? bodyData = null)
        {
            var payloadDict = new Dictionary<string, object?>();

            if (routeData != null)
            {
                var routeProps = routeData.GetType().GetProperties();
                foreach (var prop in routeProps)
                    payloadDict[prop.Name] = prop.GetValue(routeData);
            }

            if (bodyData != null)
            {
                payloadDict["Body"] = bodyData;
            }

            return JsonSerializer.Serialize(payloadDict, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }

}