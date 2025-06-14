using VidizmoBackend.Models;

namespace VidizmoBackend.Repositories
{
    public interface IVideoRepository
    {
        // Upload a video to the repository
        Task<bool> UploadVideoAsync(Video video);
        // Get a video by its ID
        Task<Video?> GetVideoByIdAsync(int videoId);
        // Delete a video from the repository
        Task<bool> DeleteVideoAsync(Video video);
    }
}