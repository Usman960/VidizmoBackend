using VidizmoBackend.Models;
using VidizmoBackend.DTOs;

namespace VidizmoBackend.Repositories {
    public interface IGroupRepository
    {
        // Create a new group
        Task<bool> CreateGroupAsync(Group group);
        // delete a group by its ID
        Task<bool> DeleteGroupAsync(Group group);
        // Get a group by its ID
        Task<Group?> GetGroupByIdAsync(int groupId);
        Task<List<GroupListDto>> GetGroupListsAsync(int orgId);
    }
}