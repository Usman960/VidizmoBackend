using VidizmoBackend.Repositories;
using VidizmoBackend.Models;
using VidizmoBackend.DTOs;

namespace VidizmoBackend.Services {
    public class GroupService: IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IOrgRepository _orgRepository;

        public GroupService(IGroupRepository groupRepository, IOrgRepository orgRepository)
        {
            _groupRepository = groupRepository;
            _orgRepository = orgRepository;
        }

        public async Task<bool> CreateGroupAsync(int orgId, int userId, string groupName)
        {
            // Check if organization exists
            var org = await _orgRepository.GetOrgByIdAsync(orgId);
            if (org == null)
            {
                return false; // Organization not found
            }

            // Create a new group
            var group = new Group
            {
                OrganizationId = orgId,
                CreatedByUserId = userId,
                Name = groupName,
                CreatedAt = DateTime.UtcNow,
            };

            // Save the group to the repository
            return await _groupRepository.CreateGroupAsync(group);
        }

        public async Task<bool> DeleteGroupAsync(int groupId)
        {
            // Check if the group exists
            var group = await _groupRepository.GetGroupByIdAsync(groupId);
            if (group == null)
            {
                return false; // Group not found
            }

            // Delete the group
            return await _groupRepository.DeleteGroupAsync(group);
        }

        public async Task<List<GroupListDto>?> GetGroupLists(int orgId)
        {
            var org = await _orgRepository.GetOrgByIdAsync(orgId);
            if (org == null) throw new ArgumentException("Organization not found");

            return await _groupRepository.GetGroupListsAsync(orgId);
        }
    }
}