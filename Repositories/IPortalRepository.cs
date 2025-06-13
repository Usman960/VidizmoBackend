using vividizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface IPortalRepository
    {
        Task<bool> CreatePortalAsync(Portal portal);
        Task<bool> DeletePortalAsync(int portalId);
        Task<Portal?> GetPortalByIdAsync(int portalId);
    }
}