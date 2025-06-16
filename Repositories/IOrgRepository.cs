using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface IOrgRepository
    {
        Task<bool> CreateOrgAsync(Organization org);
        // get organization by organization name
        Task<Organization?> GetOrgByNameAsync(string orgName);
        Task<Organization?> GetOrgByIdAsync(int orgId);
    }
}
