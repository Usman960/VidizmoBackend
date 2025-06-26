using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using VidizmoBackend.Data;
using System.Threading.Tasks;
using System.Linq;

public class TokenMiddleware
{
    private readonly RequestDelegate _next;

    public TokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        // Allow requests to endpoints that allow anonymous access
        var endpoint = context.GetEndpoint();
        var allowAnonymous = endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null;

        if (allowAnonymous)
        {
            await _next(context); // Let it pass
            return;
        }

        var headers = context.Request.Headers;

        string authHeader = headers["Authorization"];

        if (string.IsNullOrEmpty(authHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Authorization header is missing.");
            return;
        }

        if (authHeader.StartsWith("Bearer "))
        {
            // Let JWT middleware handle this
            await _next(context);
            return;
        }

        // Step 1: Hash the provided token (raw scoped token)
        var tokenHash = HashToken(authHeader.Trim());

        // Step 2: Try to fetch the scoped token from DB
        var scopedToken = await dbContext.ScopedTokens
            .Include(t => t.Organization)
            .Include(t => t.CreatedByUser)
            .FirstOrDefaultAsync(t =>
                t.TokenHash == tokenHash &&
                !t.IsRevoked &&
                t.ExpiresAt > DateTime.UtcNow);

        if (scopedToken == null)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid or expired scoped token.");
            return;
        }

        // Step 3: Create claims identity based on scoped token
        var claims = new List<Claim>
        {
            new Claim("ScopedTokenId", scopedToken.ScopedTokenId.ToString()),
            new Claim("OrganizationId", scopedToken.OrganizationId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "ScopedToken");

        context.User = new ClaimsPrincipal(identity);

        await _next(context);
    }

    private string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}
