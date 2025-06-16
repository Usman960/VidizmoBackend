using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Data;
using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public class OrgRepository : IOrgRepository
    {
        private readonly ApplicationDbContext _context;

        public OrgRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateOrgAsync(Organization org)
        {
            _context.Organizations.Add(org);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Organization?> GetOrgByNameAsync(string orgName)
        {
            return await _context.Organizations
                .FirstOrDefaultAsync(o => o.Name == orgName);
        }
        
        public Task<Organization?> GetOrgByIdAsync(int orgId) {
            // return organization by id
            return _context.Organizations
                .FirstOrDefaultAsync(o => o.OrganizationId == orgId);
        }
        // public async Task<bool> DeleteOrgAsync(int orgId)
        // {
        //     var org = await _context.Organizations.FindAsync(orgId);
        //     if (org == null) return false;

        //     _context.Organizations.Remove(org);
        //     return await _context.SaveChangesAsync() > 0;
        // }

        // // get organization by portal name
        // public async Task<Organization?> GetOrgByPortalNameAsync(string portalName)
        // {
        //     return await _context.Organizations
        //         .Include(o => o.Portals)
        //         .FirstOrDefaultAsync(o => o.Portals.Any(p => p.Name == portalName));
        // }
    }
}
