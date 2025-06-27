using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class RoleService: IRoleService
    {
        private readonly IRoleRepository _roleRepo;
        private readonly IUserRepository _userRepo;
        private readonly IGroupRepository _groupRepo;
        private readonly IOrgRepository _orgRepo;
        public RoleService(IRoleRepository roleRepo, IUserRepository userRepo, IGroupRepository groupRepo, IOrgRepository orgRepository)
        {
            _roleRepo = roleRepo;
            _userRepo = userRepo;
            _groupRepo = groupRepo;
            _orgRepo = orgRepository;
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int organizationId, int roleId, int assignedByUserId)
        {
            if (userId <= 0 || organizationId <= 0 || roleId <= 0)
            {
                throw new ArgumentException("Invalid user, organization, or role ID.");
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

        public async Task<bool> CreateRoleAsync(int orgId, int userId, RoleDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description) || dto.Permissions == null)
            {
                throw new ArgumentException("Invalid role data.");
            }


            if (await _roleRepo.RoleWithPermissionsExistsAsync(orgId, dto.Permissions)) throw new InvalidOperationException("Role with the same permissions already exists.");

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

        public async Task<bool> EditRoleAsync(int roleId, RoleDto dto)
        {
            if (roleId <= 0 || dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description) || dto.Permissions == null)
            {
                throw new ArgumentException("Invalid role data.");
            }

            if (await _roleRepo.IsRoleAssignedToAdminAsync(roleId)) throw new InvalidOperationException("Cannot edit a role that is assigned to an admin.");

            return await _roleRepo.EditRoleAsync(roleId, dto);
        }

        public async Task<bool> RevokeRoleAsync(int userId, int userOgGpRoleId)
        {
            if (userId <= 0 || userOgGpRoleId <= 0)
            {
                throw new ArgumentException("Invalid user or role assignment ID.");
            }
            var roleAssignment = await _roleRepo.GetRoleAssignment(userOgGpRoleId);
            if (roleAssignment == null) throw new ArgumentException("Invalid role assignment id");

            if (await _roleRepo.IsRoleAssignedToAdminAsync(roleAssignment.RoleId)) throw new InvalidOperationException("Cannot revoke a role that is assigned to an admin.");
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

        public async Task<RolesDto> GetAllRoles(int orgId)
        {
            var org = await _orgRepo.GetOrgByIdAsync(orgId);
            if (org == null) throw new ArgumentException("Invalid organization id");

            return await _roleRepo.GetAllRoles(orgId);
        }

        public async Task<List<IndividualRolesDto>> GetIndividualRoles(int orgId)
        {
            var org = await _orgRepo.GetOrgByIdAsync(orgId);
            if (org == null) throw new ArgumentException("Organization not found");

            return await _roleRepo.GetIndividualRoles(orgId);
        }

        public async Task<List<GroupRolesDto>> GetGroupRoles(int orgId)
        {
            var org = await _orgRepo.GetOrgByIdAsync(orgId);
            if (org == null) throw new ArgumentException("Organization not found");

            return await _roleRepo.GetGroupRoles(orgId);
        }
    }
}