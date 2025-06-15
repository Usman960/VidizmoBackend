using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Data;
using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddUserToOrganizationAsync(int userId, int organizationId)
        {
            var userOrgRole = new UserOrgRole
            {
                UserId = userId,
                OrganizationId = organizationId
            };

            _context.UserOrgRoles.Add(userOrgRole);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
