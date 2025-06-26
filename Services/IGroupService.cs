using VidizmoBackend.DTOs;

namespace VidizmoBackend.Services
{
    public interface IGroupService
    {
        Task<bool> CreateGroupAsync(int orgId, int userId, string groupName);
        Task<bool> DeleteGroupAsync(int groupId);
        Task<List<GroupListDto>?> GetGroupLists(int orgId);
    }
}