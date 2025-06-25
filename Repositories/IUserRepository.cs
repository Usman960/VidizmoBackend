using VidizmoBackend.DTOs;
using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
        Task<bool> AssociateOrganizationWithUserAsync(int userId, int organizationId);
        Task<Organization?> GetOrganizationByUserIdAsync(int userId);
        Task<User?> GetUserByIdAsync(int userId);
        // add mutliple users to a group
        Task<bool> AddUserToGroupAsync(int groupId, int userId, int currentUserId);
        // get users by their IDs
        Task<List<User>> GetUsersByIdsAsync(List<int> userIds);
        // delete users from a group
        Task<bool> RemoveUserFromGroupAsync(int groupId, int userId);
        Task<List<UserResDto>> GetUsersInOrganization(int orgId);
        Task<User?> GetUserByEmail(string email);
        Task<List<UsersNotInGroupDto>> GetUsersNotInGroup(int groupId);
        Task<User?> GetUserByGroupId(int groupId, int userId); 
    }
}
