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
        
        public async Task<List<UserWithRolesDto>> GetUsersWithRolesAsync(int orgId)
        {
            // Step 1: Get all users in this org (based on direct or group assignment)
            var users = await _context.Users
                .Where(u => _context.UserOgGpRoles.Any(r =>
                            r.OrganizationId == orgId &&
                            r.Status == "Active" &&
                            (r.UserId == u.UserId || (
                                r.GroupId != null &&
                                _context.UserGroups.Any(ug => ug.UserId == u.UserId && ug.GroupId == r.GroupId)
                            ))
                ))
                .ToListAsync();

            // Step 2: For each user, fetch direct and group-based role assignments
            var userDtos = new List<UserWithRolesDto>();

            foreach (var user in users)
            {
                // Direct roles
                var directRoles = await _context.UserOgGpRoles
                    .Where(ur =>
                        ur.OrganizationId == orgId &&
                        ur.Status == "Active" &&
                        ur.UserId == user.UserId)
                    .Include(ur => ur.Role)
                    .ToListAsync();

                // Group-based roles
                var groupIds = await _context.UserGroups
                    .Where(ug => ug.UserId == user.UserId)
                    .Select(ug => ug.GroupId)
                    .ToListAsync();

                var groupRoles = await _context.UserOgGpRoles
                    .Where(ur =>
                        ur.OrganizationId == orgId &&
                        ur.Status == "Active" &&
                        ur.GroupId != null &&
                        groupIds.Contains(ur.GroupId.Value))
                    .Include(ur => ur.Role)
                    .ToListAsync();

                // Combine both
                var allRoles = directRoles
                    .Concat(groupRoles)
                    .Select(ur => new RoleAssignments
                    {
                        UserOgGpRoleId = ur.UserOgGpRoleId,
                        RoleName = ur.Role.Name
                    })
                    .ToList();

                userDtos.Add(new UserWithRolesDto
                {
                    UserId = user.UserId,
                    Fullname = $"{user.Firstname} {user.Lastname ?? ""}",
                    Email = user.Email,
                    Roles = allRoles
                });
            }

            return userDtos;
        }

    }
}
