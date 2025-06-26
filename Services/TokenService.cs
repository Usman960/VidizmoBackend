using System.Text.Json;
using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class TokenService:ITokenService
    {
        private readonly ITokenRepository _tokenRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IUserRepository _userRepo;
        private readonly List<(string action, string entity)> _allowedScope =
            new()
            {
                ("view", "metadata"),
                ("edit", "metadata"),
                ("play", "video"),
                ("upload", "video"),
                ("download", "video")
            };

        public TokenService(ITokenRepository tokenRepo, IRoleRepository roleRepo, IUserRepository userRepo)
        {
            _tokenRepo = tokenRepo;
            _roleRepo = roleRepo;
            _userRepo = userRepo;
        }

        public async Task<string> GenerateScopedTokenAsync(int userId, int orgId, TokenDto dto)
        {
            // 1. Validate scope against allowed
            foreach (var scope in dto.Permissions.Permissions)
            {
                if (!_allowedScope.Any(p =>
                    p.action.Equals(scope.Action, StringComparison.OrdinalIgnoreCase) &&
                    p.entity.Equals(scope.Entity, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"Permission '{scope.Action}:{scope.Entity}' is not allowed in scoped tokens.");
                }
            }

            if (dto.DurationInHours < 1)
                throw new ArgumentException("Expiration duration must be at least 1 hour.");

            // 2. Check user's RBAC permissions
            var userPermissions = await _roleRepo.GetUserPermissionsAsync(userId);
            // Null check in case no permissions were found
            if (userPermissions == null || userPermissions.Permissions == null)
                throw new InvalidOperationException("User has no permissions.");

            foreach (var scope in dto.Permissions.Permissions)
            {
                if (!userPermissions.Permissions.Any(p =>
                    p.Action.Equals(scope.Action, StringComparison.OrdinalIgnoreCase) &&
                    p.Entity.Equals(scope.Entity, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException($"You are not authorized to delegate permission '{scope.Action}:{scope.Entity}'.");
                }
            }

            var expiresAt = DateTime.UtcNow.AddHours(dto.DurationInHours);
            // 3. Store
            return await _tokenRepo.CreateScopedTokenAsync(userId, orgId, dto.Permissions, expiresAt);
        }

        public async Task<bool> TokenHasPermissionAsync(int tokenId, PermissionDto permissionDto)
        {
            var token = await _tokenRepo.GetTokenByIdAsync(tokenId);
            if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow) return false;

            var scopes = JsonSerializer.Deserialize<PermissionsDto>(token.Permissions);
            return _tokenRepo.TokenHasPermissionAsync(scopes, permissionDto);
        }

        public async Task<bool> DeleteTokenAsync(int tokenId)
        {
            var token = await _tokenRepo.GetTokenByIdAsync(tokenId);
            if (token == null)
                throw new InvalidOperationException("Token not found or already deleted.");
            return await _tokenRepo.DeleteTokenAsync(token);
        }

        public async Task<bool> RevokeTokenAsync(int tokenId)
        {
            var token = await _tokenRepo.GetTokenByIdAsync(tokenId);
            if (token == null)
                throw new InvalidOperationException("Token not found or already revoked.");
            return await _tokenRepo.RevokeTokenAsync(token);
        }

        public async Task<List<TokenViewDto>> GetTokensByUserId(int userId)
        {
            if (userId <= 0) throw new ArgumentException("Invalid user Id");

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            return await _tokenRepo.GetTokensByUserIdAsync(userId); 
        }

    }
}

