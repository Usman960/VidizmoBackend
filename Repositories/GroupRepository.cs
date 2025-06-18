namespace VidizmoBackend.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using VidizmoBackend.Data;
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
    }
}