using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class OrganizationService
    {
        private readonly IOrgRepository _organizationRepo;
        private readonly IRoleRepository _roleRepo;

        public OrganizationService(IOrgRepository organizationRepo, IRoleRepository roleRepo)
        {
            _organizationRepo = organizationRepo;
            _roleRepo = roleRepo;
        }

        public async Task<bool> CreateOrganizationAsync(int userId, CreateOrgReqDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Invalid organization data.");
            }

            // check if organization name already exists
            var existingOrg = await _organizationRepo.GetOrgByNameAsync(dto.Name);
            if (existingOrg != null)
            {
                throw new InvalidOperationException("Organization with this name already exists.");
            }

            // Check if the user already belongs to an organization
            if (await _organizationRepo.GetOrgByUserIdAsync(userId) != null)
            {
                throw new InvalidOperationException("User already belongs to an organization.");
            }

            var org = new Organization
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId,
            };

            if (!await _organizationRepo.CreateOrgAsync(org))
            {
                throw new InvalidOperationException("Failed to create organization.");
            }

            var role = await _roleRepo.CreateAdminRoleAsync(org.OrganizationId, userId);

            // Assign the admin role to the user
            var userOrgRole = new UserOrgRole
            {
                UserId = userId,
                OrganizationId = org.OrganizationId,
                RoleId = role.RoleId,
                AssignedAt = DateTime.UtcNow,
                AssignedByUserId = userId,
                Status = "Active"
            };

            if (!await _roleRepo.AssignRoleToUserAsync(userOrgRole))
            {
                throw new InvalidOperationException("Failed to assign role to user.");
            }
            return true;
        }        
    }
}
