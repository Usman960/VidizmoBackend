namespace VidizmoBackend.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using VidizmoBackend.Data;
    using VidizmoBackend.DTOs;
    using VidizmoBackend.Models;

    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public GroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateGroupAsync(Group group)
        {
            _context.Groups.Add(group);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteGroupAsync(Group group)
        {
            // delete all users in the group
            var userGroups = _context.UserGroups.Where(ug => ug.GroupId == group.GroupId);
            _context.UserGroups.RemoveRange(userGroups);
            // delete the group
            _context.Groups.Remove(group);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Group?> GetGroupByIdAsync(int groupId)
        {
            return await _context.Groups
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<List<GroupListDto>> GetGroupListsAsync(int orgId)
        {
            var groups = await _context.Groups
                .Where(g => g.OrganizationId == orgId)
                .ToListAsync();

            var result = new List<GroupListDto>();

            foreach (var group in groups)
            {
                // Users in this group
                var userList = await _context.UserGroups
                    .Where(ug => ug.GroupId == group.GroupId)
                    .Include(ug => ug.User)
                    .Select(ug => new GroupUserDto
                    {
                        UserId = ug.User.UserId,
                        Email = ug.User.Email
                    })
                    .ToListAsync();

                // Active roles assigned to this group
                var groupRoles = await _context.UserOgGpRoles
                    .Where(r => r.GroupId == group.GroupId &&
                                r.OrganizationId == orgId &&
                                r.Status == "Active")
                    .Include(r => r.Role)
                    .Select(r => new GroupRoleDto
                    {
                        UserOgGpRoleId = r.UserOgGpRoleId,
                        RoleId = r.Role.RoleId,
                        RoleName = r.Role.Name
                    })
                    .Distinct() // Optional: prevent duplicate roles if same role is assigned multiple times
                    .ToListAsync();

                result.Add(new GroupListDto
                {
                    GroupId = group.GroupId,
                    GroupName = group.Name,
                    Users = userList,
                    Roles = groupRoles
                });
            }

            return result;
        }
    }
}