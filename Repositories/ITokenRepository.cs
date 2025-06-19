using Microsoft.OpenApi.Writers;
using VidizmoBackend.DTOs;
using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface ITokenRepository
    {
        Task<string> CreateScopedTokenAsync(int createdByUserId, int orgId, PermissionsDto scopes, DateTime expiresAt);
        bool TokenHasPermissionAsync(PermissionsDto scope, PermissionDto permissionDto);
        Task<ScopedToken?> GetTokenByIdAsync(int tokenId);
        Task<bool> DeleteTokenAsync(ScopedToken token);
        Task<bool> RevokeTokenAsync(ScopedToken token);
    }
}

