using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Data;
using VidizmoBackend.Models;
using VidizmoBackend.DTOs;

namespace VidizmoBackend.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> RoleWithAllPermissionsExistsAsync(int organizationId)
        {
            // fetch all roles for the organization from UserOrdRole table
            var roles = await _context.UserOrgRoles
                .Include(uor => uor.Role)
                .Where(uor => uor.OrganizationId == organizationId)
                .Select(uor => uor.Role)
                .ToListAsync();

            if (roles == null || roles.Count == 0) return null;

            // check if any role has all permissions in RolePermissions table
            foreach (var role in roles)
            {
                var permissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == role.RoleId)
                    .ToListAsync();

                // if permissions count is equal to the total number of rows in Permissions table, return role
                if (permissions.Count == _context.Permissions.Count())
                {
                    return role;
                }
                else continue;
            }
            return null;
        }

        public async Task<bool> AssignRoleToUserAsync(UserOrgRole userOrgRole)
        {
            _context.UserOrgRoles.Add(userOrgRole);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Role> CreateRoleAsync(Role role, PermissionsDto permissionsDto)
        {
            // Add role first to get its ID
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // Add matching permissions from DB
            foreach (var permissionDto in permissionsDto.Permissions)
            {
                var dbPermission = await _context.Permissions
                    .FirstOrDefaultAsync(p =>
                        p.Action.ToLower() == permissionDto.Action.ToLower() &&
                        p.Entity.ToLower() == permissionDto.Entity.ToLower());

                if (dbPermission == null)
                    throw new InvalidOperationException($"Permission '{permissionDto.Action}:{permissionDto.Entity}' not found in database.");

                var rolePermission = new RolePermission
                {
                    RoleId = role.RoleId,
                    PermissionId = dbPermission.PermissionId
                };

                _context.RolePermissions.Add(rolePermission);
            }

            return role;
        }

        public async Task<Role> CreateAdminRoleAsync(int organizationId, int userId)
        {
            var adminRole = new Role
            {
                Name = "Admin",
                Description = "Administrator role with full permissions",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
            };

            _context.Roles.Add(adminRole);
            await _context.SaveChangesAsync();

            var allPermissions = await _context.Permissions.ToListAsync();

            foreach (var permission in allPermissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.RoleId,
                    PermissionId = permission.PermissionId
                });
            }
            await _context.SaveChangesAsync();

            return adminRole;
        }
    }
}