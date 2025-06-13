using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Data;
using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<bool> CreateRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AssignRoleAsync(UserPortalRole userPortalRole)
        {
            _context.UserPortalRoles.Add(userPortalRole);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Role?> GetRoleByExactPermissionsAsync(Permissions dto)
        {
            // Get all roles
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            foreach (var role in roles)
            {
                var rolePermissions = role.RolePermissions
                    .Select(rp => new { rp.Permission.Action, rp.Permission.Entity })
                    .ToList();

                var inputPermissions = dto.Permissions
                    .Select(p => new { p.Action.ToLower(), p.Entity.ToLower() })
                    .ToList();

                // Compare counts first
                if (rolePermissions.Count != inputPermissions.Count)
                    continue;

                // Compare sets
                bool isExactMatch = !inputPermissions.Except(rolePermissions.Select(p => new { Action = p.Action.ToLower(), Entity = p.Entity.ToLower() })).Any();

                if (isExactMatch)
                    return role;
            }

            return null;
        }
    }
}