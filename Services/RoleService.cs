using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepo;
        private readonly IUserRepository _userRepo;
        private readonly IGroupRepository _groupRepo;
        public RoleService(IRoleRepository roleRepo, IUserRepository userRepo, IGroupRepository groupRepo)
        {
            _roleRepo = roleRepo;
            _userRepo = userRepo;
            _groupRepo = groupRepo;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int organizationId, int roleId, int assignedByUserId)
        {
            if (userId <= 0 || organizationId <= 0 || roleId <= 0)
            {
                throw new ArgumentException("Invalid user, organization, or role ID.");
            }
            var role = await _roleRepo.RoleExistsInOrganizationAsync(organizationId, roleId);
            if (role == null)
            {
                throw new ArgumentException("Role does not exist in the specified organization.");
            }
            var userOgGpRole = new UserOgGpRole
            {
                UserId = userId,
                OrganizationId = organizationId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = assignedByUserId,
                Status = "Active"
            };
            return await _roleRepo.AssignRoleAsync(userOgGpRole);
        }

        public async Task<bool> CreateRoleAsync(int userId, RoleDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description) || dto.Permissions == null)
            {
                throw new ArgumentException("Invalid role data.");
            }

            var org = await _userRepo.GetOrganizationByUserIdAsync(userId);

            if (await _roleRepo.RoleWithPermissionsExistsAsync(org.OrganizationId, dto.Permissions)) throw new InvalidOperationException("Role with the same permissions already exists.");

            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
            };

            return await _roleRepo.CreateRoleAsync(role, dto.Permissions);
        }

        public async Task<bool> UserHasPermissionAsync(int userId, PermissionDto permissionDto)
        {
            if (userId <= 0 || permissionDto == null)
            {
                throw new ArgumentException("Invalid user ID or permission data.");
            }
            return await _roleRepo.UserHasPermissionAsync(userId, permissionDto);
        }

        public async Task<bool> AssignRoleToGroupAsync(int groupId, int organizationId, int roleId, int assignedByUserId)
        {
            if (groupId <= 0 || organizationId <= 0 || roleId <= 0)
            {
                throw new ArgumentException("Invalid group, organization, or role ID.");
            }
            var role = await _roleRepo.RoleExistsInOrganizationAsync(organizationId, roleId);
            if (role == null)
            {
                throw new ArgumentException("Role does not exist in the specified organization.");
            }
            var userOgGpRole = new UserOgGpRole
            {
                GroupId = groupId,
                OrganizationId = organizationId,
                RoleId = roleId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = assignedByUserId,
                Status = "Active"
            };
            return await _roleRepo.AssignRoleAsync(userOgGpRole);
        }

        public async Task<bool> EditRoleAsync(int userId, int roleId, RoleDto dto)
        {
            if (roleId <= 0 || dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description) || dto.Permissions == null)
            {
                throw new ArgumentException("Invalid role data.");
            }

            var org = await _userRepo.GetOrganizationByUserIdAsync(userId);

            var existingRole = await _roleRepo.RoleExistsInOrganizationAsync(org.OrganizationId, roleId);
            if (existingRole == null)
            {
                throw new InvalidOperationException("Role not found.");
            }

            return await _roleRepo.EditRoleAsync(existingRole.RoleId, dto);
        }

        public async Task<bool> RevokeRoleAsync(int userId, int userOgGpRoleId)
        {
            if (userId <= 0 || userOgGpRoleId <= 0)
            {
                throw new ArgumentException("Invalid user or role assignment ID.");
            }
            return await _roleRepo.RevokeRoleAsync(userId, userOgGpRoleId);
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            if (await _roleRepo.IsRoleAssignedToAdminAsync(roleId))
            {
                throw new InvalidOperationException("Cannot delete a role that is assigned to an admin.");
            }
            return await _roleRepo.DeleteRoleAsync(roleId);
        }

        public async Task<bool> DeleteAssignmentsByGroupId(int groupId)
        {
            if (await _groupRepo.GetGroupByIdAsync(groupId) == null)
            {
                throw new ArgumentException("Group not found.");
            }
            return await _roleRepo.DeleteAssignmentsByGroupId(groupId);
        }
    }
}