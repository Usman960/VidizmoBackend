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
            var roles = await _context.UserOgGpRoles
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

        public async Task<bool> AssignRoleToUserAsync(UserOgGpRole UserOgGpRole)
        {
            _context.UserOgGpRoles.Add(UserOgGpRole);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateRoleAsync(Role role, PermissionsDto permissionsDto)
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

            return await _context.SaveChangesAsync() > 0;
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

        public async Task<bool> DeleteAllRoleAssignmentsAsync(int userId)
        {
            // Find all role assignments for the user
            var roleAssignments = await _context.UserOgGpRoles
                .Where(uor => uor.UserId == userId)
                .ToListAsync();

            if (roleAssignments.Count == 0) return true;

            // Remove all role assignments
            _context.UserOgGpRoles.RemoveRange(roleAssignments);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UserHasPermissionAsync(int userId, PermissionDto permissionDto) 
        {
            // fetch roles from UserOgGpRole table for the user where status is Active
            var userRoles = await _context.UserOgGpRoles
                .Where(uor => uor.UserId == userId && uor.Status == "Active")
                .Select(uor => uor.Role)
                .ToListAsync();
            
            if (userRoles == null || userRoles.Count == 0) return false;

            // check if any role has the permission in RolePermissions table
            foreach (var role in userRoles)
            {
                var hasPermission = await _context.RolePermissions
                    .AnyAsync(rp => rp.RoleId == role.RoleId &&
                                    rp.Permission.Action.ToLower() == permissionDto.Action.ToLower() &&
                                    rp.Permission.Entity.ToLower() == permissionDto.Entity.ToLower());

                if (hasPermission)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<Role?> RoleExistsInOrganizationAsync(int organizationId, int roleId)
        {
            // return the role where the organizationId of the user who created the role matches with the organizationId given in arg
            return await _context.Roles
                .Where(r => r.RoleId == roleId && r.CreatedByUser.OrganizationId == organizationId)
                .FirstOrDefaultAsync();
        }
    }
}