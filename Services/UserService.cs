using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrgRepository _orgRepository;
        private readonly IGroupRepository _groupRepository;

        public UserService(IUserRepository userRepository, IOrgRepository orgRepository, IGroupRepository groupRepository)
        {
            _userRepository = userRepository;
            _orgRepository = orgRepository;
            _groupRepository = groupRepository;
        }

        // Add a user to an organization
        public async Task<bool> AddUserToOrganizationAsync(int userId, int organizationId)
        {
            if (userId <= 0 || organizationId <= 0)
            {
                throw new ArgumentException("Invalid user or organization ID.");
            }
            var org = await _orgRepository.GetOrgByIdAsync(organizationId);
            if (org == null)
                throw new ArgumentException("Organization not found.");
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");
            if (user.OrganizationId.HasValue)
                throw new InvalidOperationException("User already belongs to an organization.");

            return await _userRepository.AssociateOrganizationWithUserAsync(userId, organizationId);
        }

        public async Task<bool> AddUsersToGroupAsync(int groupId, List<int> userIds, int userId)
        {
            if (groupId <= 0 || userIds == null || userIds.Count == 0 || userId <= 0)
            {
                throw new ArgumentException("Invalid group or user IDs.");
            }

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new ArgumentException("Group not found.");

            var users = await _userRepository.GetUsersByIdsAsync(userIds);
            if (users.Count != userIds.Count)
                throw new ArgumentException("No valid users found.");

            return await _userRepository.AddUsersToGroupAsync(groupId, users, userId);
        }

        public async Task<bool> RemoveUsersFromGroupAsync(int groupId, List<int> userIds)
        {
            if (groupId <= 0 || userIds == null || userIds.Count == 0)
            {
                throw new ArgumentException("Invalid group or user IDs.");
            }

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new ArgumentException("Group not found.");

            var users = await _userRepository.GetUsersByIdsAsync(userIds);
            if (users.Count != userIds.Count)
                throw new ArgumentException("No valid users found.");

            return await _userRepository.RemoveUsersFromGroupAsync(groupId, users);
        }

        public async Task<List<UserWithRolesDto>> GetUsersWithRolesAsync(int orgId)
        {
            var org = await _orgRepository.GetOrgByIdAsync(orgId);
            if (org == null)
                throw new ArgumentException("Organization not found.");

            return await _userRepository.GetUsersWithRolesAsync(orgId);
        }
    }
}