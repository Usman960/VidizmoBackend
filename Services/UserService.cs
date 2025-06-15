using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Add a user to an organization
        public async Task<bool> AddUserToOrganizationAsync(int userId, int organizationId)
        {
            if (userId <= 0 || organizationId <= 0)
            {
                throw new ArgumentException("Invalid user or organization ID.");
            }
            return await _userRepository.AddUserToOrganizationAsync(userId, organizationId);
        }
    }
}