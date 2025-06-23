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
        Task<bool> AddUsersToGroupAsync(int groupId, List<User> users, int userId);
        // get users by their IDs
        Task<List<User>> GetUsersByIdsAsync(List<int> userIds);
        // delete users from a group
        Task<bool> RemoveUsersFromGroupAsync(int groupId, List<User> users);
        Task<List<UserWithRolesDto>> GetUsersWithRolesAsync(int orgId);
    }
}
