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

        public async Task<bool> AssociateOrganizationWithUserAsync(int userId, int organizationId)
        {
            // set organizationId for the user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }
            user.OrganizationId = organizationId;
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Organization?> GetOrganizationByUserIdAsync(int userId)
        {
            return await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => u.Organization)
                .FirstOrDefaultAsync();
        }
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<bool> AddUsersToGroupAsync(int groupId, List<User> users, int userId)
        {
            // filter out users that are already in the group
            var existingUserGroups = await _context.UserGroups
                .Where(ug => ug.GroupId == groupId && users.Select(u => u.UserId).Contains(ug.UserId))
                .ToListAsync();

            // skip users that are already in the group
            var newUsers = users
                .Where(u => !existingUserGroups.Any(ug => ug.UserId == u.UserId))
                .Select(u => new UserGroup
                {
                    UserId = u.UserId,
                    GroupId = groupId,
                    AddedById = userId,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();
        
            _context.UserGroups.AddRange(newUsers);
            return await _context.SaveChangesAsync() > 0; // Save changes and return success status
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<int> userIds)
        {
            return await _context.Users
                .Where(u => userIds.Contains(u.UserId))
                .ToListAsync();
        }
        
        public async Task<bool> RemoveUsersFromGroupAsync(int groupId, List<User> users)
        {
            var userGroups = await _context.UserGroups
                .Where(ug => ug.GroupId == groupId && users.Select(u => u.UserId).Contains(ug.UserId))
                .ToListAsync();

            if (userGroups.Count == 0)
            {
                return false; // No matching user groups found
            }

            _context.UserGroups.RemoveRange(userGroups);
            return await _context.SaveChangesAsync() > 0; // Save changes and return success status
        }
    }
}
