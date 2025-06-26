using VidizmoBackend.DTOs;

namespace VidizmoBackend.Services
{
    public interface IOrganizationService
    {
        Task<bool> CreateOrganizationAsync(int userId, CreateOrgReqDto dto);
        Task<string?> GetOrgNameById(int orgId);
    }
}