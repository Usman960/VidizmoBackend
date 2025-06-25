using Microsoft.EntityFrameworkCore;
using VidizmoBackend.Data;
using VidizmoBackend.Models;
using VidizmoBackend.DTOs;

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

        public async Task<bool> AddUserToGroupAsync(int groupId, int userId, int currentUserId)
        {
            var newUser = new UserGroup
            {
                UserId = userId,
                GroupId = groupId,
                AddedById = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserGroups.Add(newUser);
            return await _context.SaveChangesAsync() > 0; // Save changes and return success status
        }

        public async Task<List<User>> GetUsersByIdsAsync(List<int> userIds)
        {
            return await _context.Users
                .Where(u => userIds.Contains(u.UserId))
                .ToListAsync();
        }

        public async Task<bool> RemoveUserFromGroupAsync(int groupId, int userId)
        {
            var userGroup = await _context.UserGroups
                .Where(ug => ug.GroupId == groupId && ug.UserId == userId)
                .FirstOrDefaultAsync();

            _context.UserGroups.Remove(userGroup);
            return await _context.SaveChangesAsync() > 0; // Save changes and return success status
        }

        public async Task<List<UserResDto>> GetUsersInOrganization(int orgId)
        {
            return await _context.Users
                .Where(u => u.OrganizationId == orgId)
                .Select(u => new UserResDto
                {
                    UserId = u.UserId,
                    Fullname = $"{u.Firstname} {u.Lastname ?? ""}",
                    Email = u.Email
                })
                .ToListAsync();
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UsersNotInGroupDto>> GetUsersNotInGroup(int groupId)
        {
            var userIdsInGroup = await _context.UserGroups
                .Where(ug => ug.GroupId == groupId)
                .Select(ug => ug.UserId)
                .ToListAsync();

            var orgId = await _context.Groups
                .Where(g => g.GroupId == groupId)
                .Select(g => g.OrganizationId)
                .FirstOrDefaultAsync();

            var usersNotInGroup = await _context.Users
                .Where(u => !userIdsInGroup.Contains(u.UserId) && orgId == u.OrganizationId)
                .Select(u => new UsersNotInGroupDto
                {
                    UserId = u.UserId,
                    Email = u.Email
                })
                .ToListAsync();

            return usersNotInGroup;
        }

        public async Task<User?> GetUserByGroupId(int groupId, int userId)
        {
            return await _context.UserGroups
                .Where(ug => ug.UserId == userId && ug.GroupId == groupId)
                .Select(ug => ug.User)
                .FirstOrDefaultAsync();
        }

    }
}
