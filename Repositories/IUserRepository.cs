using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(User user);
        Task<bool> AssociateOrganizationWithUserAsync(int userId, int organizationId);
    }
}
