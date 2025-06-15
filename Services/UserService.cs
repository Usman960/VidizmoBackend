using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrgRepository _orgRepository;
        public UserService(IUserRepository userRepository, IOrgRepository orgRepository)
        {
            _userRepository = userRepository;
            _orgRepository = orgRepository;
        }

        // Add a user to an organization
        public async Task<bool> AddUserToOrganizationAsync(int userId, int organizationId)
        {
            if (userId <= 0 || organizationId <= 0)
            {
                throw new ArgumentException("Invalid user or organization ID.");
            }
            // Check if the user already belongs to an organization
            if (await _orgRepository.GetOrgByUserIdAsync(userId) != null)
            {
                throw new InvalidOperationException("User already belongs to an organization.");
            }
            return await _userRepository.AssociateOrganizationWithUserAsync(userId, organizationId);
        }
    }
}