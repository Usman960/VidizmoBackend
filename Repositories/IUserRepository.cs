using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
        // add user to an organization
        Task<bool> AddUserToOrganizationAsync(int userId, int organizationId);
    }
}
