using System.Security.Claims;
using VidizmoBackend.Services;

namespace VidizmoBackend.Middlewares
{
    public class AuditLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AuditLogService auditLogService)
        {
            int? tokenId = null;

            int? userId = null;
            var userIdClaim = context.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int uid)) userId = uid;

            var tokenIdClaim = context.User?.Claims.FirstOrDefault(c => c.Type == "ScopedTokenId")?.Value;
            if (int.TryParse(tokenIdClaim, out int tid)) tokenId = tid;

            // Request Body
            context.Request.EnableBuffering();
            string requestBody = "";
            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Capture response
            var originalBody = context.Response.Body;
            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            bool isDownload = context.Request.Path.StartsWithSegments("/api/video/download", StringComparison.OrdinalIgnoreCase);
            bool isBinary = context.Response.ContentType?.Contains("application/octet-stream", StringComparison.OrdinalIgnoreCase) == true;
            bool isSuccess = context.Response.StatusCode == StatusCodes.Status200OK;

            string responseBody;

            if (isDownload && isBinary && isSuccess)
            {
                responseBody = $"[Binary response skipped for play/download. Content-Type: {context.Response.ContentType}]";
            }
            else
            {
                using var reader = new StreamReader(context.Response.Body, leaveOpen: true);
                responseBody = await reader.ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);
            }

            var auditLog = new AuditLog
            {
                UserId = userId,
                TokenId = tokenId,
                RequestUrl = $"{context.Request.Method} {context.Request.Path}{context.Request.QueryString}",
                RequestBody = requestBody,
                ResponseBody = responseBody,
                Timestamp = DateTime.UtcNow
            };

            await auditLogService.SendLogAsync(auditLog);
            await newBody.CopyToAsync(originalBody);
        }
    }

}