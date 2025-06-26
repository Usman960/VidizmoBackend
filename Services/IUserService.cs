using VidizmoBackend.DTOs;

namespace VidizmoBackend.Services
{
    public interface IUserService
    {
        Task<bool> AddUserToOrganizationAsync(int organizationId, string email);
        Task<bool> AddUserToGroupAsync(int groupId, int userId, int currentUserId);
        Task<bool> RemoveUserFromGroupAsync(int groupId, int userId);
        Task<List<UserResDto>> GetUsersInOrganization(int orgId);
        Task<List<UsersNotInGroupDto>> GetUsersNotInGroup(int groupId);
    }
}