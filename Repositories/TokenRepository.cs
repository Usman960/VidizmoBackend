using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Data;
using VidizmoBackend.Models;
using VidizmoBackend.DTOs;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace VidizmoBackend.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDbContext _context;

        public TokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateScopedTokenAsync(int createdByUserId, int orgId, PermissionsDto scopes, DateTime expiresAt)
        {
            var rawToken = Guid.NewGuid().ToString("N");
            var hashed = HashToken(rawToken);
            var scopeJson = JsonSerializer.Serialize(scopes);
            Console.WriteLine($"Creating token for user {createdByUserId} in organization {orgId} with scopes: {scopeJson}");
            var token = new ScopedToken
            {
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                CreatedByUserId = createdByUserId,
                OrganizationId = orgId,
                TokenHash = hashed,
                ScopeJson = scopeJson
            };

            _context.ScopedTokens.Add(token);
            await _context.SaveChangesAsync();

            return rawToken; // return only to caller
        }

        private string HashToken(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool TokenHasPermissionAsync(PermissionsDto scope, PermissionDto permissionDto)
        {
            return scope.Permissions.Any(p =>
                p.Action.Equals(permissionDto.Action, StringComparison.OrdinalIgnoreCase) &&
                p.Entity.Equals(permissionDto.Entity, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<ScopedToken?> GetTokenByIdAsync(int tokenId)
        {
            return await _context.ScopedTokens.FindAsync(tokenId);
        }

    }
}


