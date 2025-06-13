using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface IRoleRepository
    {
        Task<Role?> GetRoleByIdAsync(int roleId);
        Task<bool> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<bool> AssignRoleAsync(int userId, int roleId);
    }
}