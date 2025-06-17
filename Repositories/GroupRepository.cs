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

        public async Task<bool> DeleteGroupAsync(int groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);
            if (group == null)
            {
                return false; // Group not found
            }

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