using VidizmoBackend.DTOs;
using VidizmoBackend.Models;

namespace VidizmoBackend.Services
{
    public interface IOrganizationService
    {
        Task<User> CreateOrganizationAsync(int userId, CreateOrgReqDto dto);
        Task<string?> GetOrgNameById(int orgId);
    }
}