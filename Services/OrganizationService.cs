using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class OrganizationService
    {
        private readonly IOrgRepository _organizationRepo;
        private readonly IRoleRepository _portalRepository;

        public OrganizationService(IOrgRepository organizationRepo, IRoleRepository portalRepository)
        {
            _organizationRepo = organizationRepo;
            _portalRepository = portalRepository;
        }

        public async Task<bool> CreateOrganizationAsync(int userId, CreateOrgReqDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Invalid organization data.");
            }

            // Check if the user already belongs to an organization
            var existingOrg = await _organizationRepo.GetOrgByUserIdAsync(userId);
            if (existingOrg != null)
            {
                throw new InvalidOperationException("User already belongs to an organization.");
            }

            var org = new Organization
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
                CreatedByUserId = userId,
            };
            
            if (!await _organizationRepo.CreateOrgAsync(org))
            {
                throw new InvalidOperationException("Failed to create organization.");
            }

             
        }        
    }
}
