using VidizmoBackend.Data;
using VidizmoBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace VidizmoBackend.Repositories
{
    public class VideoRepository : IVideoRepository
    {
        private readonly ApplicationDbContext _context;

        public VideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> UploadVideoAsync(Video video)
        {
            _context.Videos.Add(video);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<Video?> GetVideoByIdAsync(int videoId)
        {
            return await _context.Videos
                .FirstOrDefaultAsync(v => v.VideoId == videoId);
        }
        public async Task<bool> DeleteVideoAsync(Video video)
        {
            _context.Videos.Remove(video);
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
