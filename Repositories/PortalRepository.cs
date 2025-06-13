using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Data;
using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public class PortalRepository : IPortalRepository
    {
        private readonly ApplicationDbContext _context;

        public PortalRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreatePortalAsync(Portal portal)
        {
            _context.Portals.Add(portal);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePortalAsync(int portalId)
        {
            var portal = await _context.Portals.FindAsync(portalId);
            if (portal == null) return false;

            _context.Portals.Remove(portal);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Portal?> GetPortalByIdAsync(int portalId)
        {
            return await _context.Portals.FindAsync(portalId);
        }

        // check if portal name exists in the organization
        public async Task<bool> PortalNameExistsInOrgAsync(string portalName, int orgId)
        {
            return await _context.Portals
                .AnyAsync(p => p.Name == portalName && p.OrganizationId == orgId);
        }
    }
}