using VidizmoBackend.Models;
using VidizmoBackend.DTOs;

namespace VidizmoBackend.Repositories
{
    public interface IRoleRepository
    {
        // check if a role with all permissions exists for the organization
        Task<Role?> RoleWithAllPermissionsExistsAsync(int organizationId);
        // assign role to user
        Task<bool> AssignRoleAsync(UserOgGpRole UserOgGpRole);
        // create role takes role and list of permissions as arguments
        Task<bool> CreateRoleAsync(Role role, PermissionsDto permissionsDto);
        // create admin role for organization
        Task<Role> CreateAdminRoleAsync(int organizationId, int userId);
        // delete role assignment
        Task<bool> DeleteAllRoleAssignmentsAsync(int userId);
        // check if user has pemissions
        Task<bool> UserHasPermissionAsync(int userId, PermissionDto permissionDto);
        // check if roles exists in organization
        Task<Role?> RoleExistsInOrganizationAsync(int organizationId, int roleId);
        // check if role with given permissions already exists in organization
        Task<bool> RoleWithPermissionsExistsAsync(int organizationId, PermissionsDto permissionsDto);
        // edit role
        Task<bool> EditRoleAsync(int roleId, RoleDto dto);
        // delete role
        Task<bool> DeleteRoleAsync(int roleId);
        // revoke role
        Task<bool> RevokeRoleAsync(int userId, int userOgGpRoleId);
        // Delete all role assignments by group Id
        Task<bool> DeleteAssignmentsByGroupId(int groupId);
        // check if role is assigned to admin
        Task<bool> IsRoleAssignedToAdminAsync(int roleId);
        // get user permissions by user id
        Task<PermissionsDto?> GetUserPermissionsAsync(int userId);
    }
}