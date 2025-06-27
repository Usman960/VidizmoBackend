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
            int? userId = null;
            int? tokenId = null;

            var userIdClaim = context.User?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (int.TryParse(userIdClaim, out int uid)) userId = uid;

            var tokenIdClaim = context.User?.Claims.FirstOrDefault(c => c.Type == "scopedtokenid")?.Value;
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
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

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