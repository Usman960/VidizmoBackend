using VidizmoBackend.DTOs;

namespace VidizmoBackend.Services
{
    public interface IRoleService
    {
        Task<bool> AssignRoleToUserAsync(int userId, int organizationId, int roleId, int assignedByUserId);
        Task<bool> CreateRoleAsync(int orgId, int userId, RoleDto dto);
        Task<bool> UserHasPermissionAsync(int userId, PermissionDto permissionDto);
        Task<bool> AssignRoleToGroupAsync(int groupId, int organizationId, int roleId, int assignedByUserId);
        Task<bool> EditRoleAsync(int roleId, RoleDto dto);
        Task<bool> RevokeRoleAsync(int userId, int userOgGpRoleId);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<bool> DeleteAssignmentsByGroupId(int groupId);
        Task<RolesDto> GetAllRoles(int orgId);
        Task<List<IndividualRolesDto>> GetIndividualRoles(int orgId);
        Task<List<GroupRolesDto>> GetGroupRoles(int orgId);
    }
}