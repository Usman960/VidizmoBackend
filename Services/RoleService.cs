using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepo;

        public RoleService(IRoleRepository roleRepo)
        {
            _roleRepo = roleRepo;
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
            return await _roleRepo.AssignRoleToUserAsync(userOgGpRole);
        }

        public async Task<bool> CreateRoleAsync(int userId, CreateRoleDto dto) {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description) || dto.Permissions == null)
            {
                throw new ArgumentException("Invalid role data.");
            }

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
    }
}