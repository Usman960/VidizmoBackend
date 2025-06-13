// using Microsoft.EntityFrameworkCore;
// using VidizmoBackend.Data;
// using VidizmoBackend.Models;

// namespace VidizmoBackend.Repositories
// {
//     public class RoleRepository : IRoleRepository
//     {
//         private readonly ApplicationDbContext _context;

//         public RoleRepository(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         public async Task<bool> CreateRoleAsync(Role role)
//         {
//             _context.Roles.Add(role);
//             return await _context.SaveChangesAsync() > 0;
//         }
//     }
// }