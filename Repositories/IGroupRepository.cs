using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories {
    public interface IGroupRepository {
        // Create a new group
        Task<bool> CreateGroupAsync(Group group);
        // delete a group by its ID
        Task<bool> DeleteGroupAsync(int groupId);
        // Get a group by its ID
        Task<Group?> GetGroupByIdAsync(int groupId);
    }
}