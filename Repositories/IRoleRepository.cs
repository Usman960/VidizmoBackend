using VidizmoBackend.Models;
using VidizmoBackend.DTOs;

namespace VidizmoBackend.Repositories
{
    public interface IRoleRepository
    {
        // check if a role with all permissions exists for the organization
        Task<Role?> RoleWithAllPermissionsExistsAsync(int organizationId);
        // assign role to user
        Task<bool> AssignRoleToUserAsync(UserOrgRole userOrgRole);
        // create role takes role and list of permissions as arguments
        Task<Role> CreateRoleAsync(Role role, PermissionsDto permissionsDto);
        // create admin role for organization
        Task<Role> CreateAdminRoleAsync(int organizationId, int userId);
    }
}