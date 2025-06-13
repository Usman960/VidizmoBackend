using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface IOrgRepository
    {
        Task<bool> CreateOrgAsync(Organization org);
        Task<bool> DeleteOrgAsync(int orgId);
        Task<Organization?> GetOrgByUserIdAsync(int userId);
    }
}
