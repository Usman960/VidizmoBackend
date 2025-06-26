using VidizmoBackend.DTOs;

namespace VidizmoBackend.Services
{
    public interface ITokenService
    {
        Task<string> GenerateScopedTokenAsync(int userId, int orgId, TokenDto dto);
        Task<bool> TokenHasPermissionAsync(int tokenId, PermissionDto permissionDto);
        Task<bool> DeleteTokenAsync(int tokenId);
        Task<bool> RevokeTokenAsync(int tokenId);
        Task<List<TokenViewDto>> GetTokensByUserId(int userId);
    }
}