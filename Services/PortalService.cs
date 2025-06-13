using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class PortalService
    {
        private readonly IPortalRepository _portalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOrgRepository _orgRepository;

        public PortalService(IPortalRepository portalRepository, IUserRepository userRepository, IOrgRepository orgRepository)
        {
            _portalRepository = portalRepository;
            _userRepository = userRepository;
            _orgRepository = orgRepository;
        }

        public async Task<bool> CreatePortalAsync(string portalName, int orgId, int userId)
        {
            // Validate portal name
            if (string.IsNullOrWhiteSpace(portalName))
            {
                throw new ArgumentException("Portal name cannot be empty.");
            }

            // check if portal already exists in the organization
            if (await _portalRepository.PortalNameExistsInOrgAsync(portalName, orgId))
            {
                throw new InvalidOperationException("Portal with this name already exists in the organization.");
            }


            // Create new portal
            var portal = new Portal
            {
                Name = portalName,
                CreatedAt = DateTime.UtcNow
            };

            return await _portalRepository.CreatePortalAsync(portal);
        }
    }
}