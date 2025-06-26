using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class UserService: IUserService
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
        public async Task<bool> AddUserToOrganizationAsync(int organizationId, string email)
        {
            if (String.IsNullOrEmpty(email) || organizationId <= 0)
            {
                throw new ArgumentException("Invalid user or organization ID.");
            }
            var org = await _orgRepository.GetOrgByIdAsync(organizationId);
            if (org == null)
                throw new ArgumentException("Organization not found.");
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                throw new ArgumentException("User not found.");
            if (user.OrganizationId.HasValue)
                throw new InvalidOperationException("User already belongs to an organization.");

            return await _userRepository.AssociateOrganizationWithUserAsync(user.UserId, organizationId);
        }

        public async Task<bool> AddUserToGroupAsync(int groupId, int userId, int currentUserId)
        {
            if (groupId <= 0 || userId <= 0|| currentUserId <= 0)
            {
                throw new ArgumentException("Invalid group or user IDs.");
            }

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new ArgumentException("Group not found.");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            user = await _userRepository.GetUserByGroupId(groupId, userId);
            if (user != null) throw new ArgumentException("User is already part of group");

            return await _userRepository.AddUserToGroupAsync(groupId, userId, currentUserId);
        }

        public async Task<bool> RemoveUserFromGroupAsync(int groupId, int userId)
        {
            if (groupId <= 0 || userId <= 0)
            {
                throw new ArgumentException("Invalid group or user ID.");
            }

            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
                throw new ArgumentException("Group not found.");

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found.");

            return await _userRepository.RemoveUserFromGroupAsync(groupId, userId);
        }

        public async Task<List<UserResDto>> GetUsersInOrganization(int orgId)
        {
            var org = await _orgRepository.GetOrgByIdAsync(orgId);
            if (org == null)
                throw new ArgumentException("Organization not found.");

            return await _userRepository.GetUsersInOrganization(orgId);
        }

        public async Task<List<UsersNotInGroupDto>> GetUsersNotInGroup(int groupId)
        {
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null) throw new ArgumentException("Group not found.");

            return await _userRepository.GetUsersNotInGroup(groupId);
        }
    }
}