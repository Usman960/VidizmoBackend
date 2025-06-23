using VidizmoBackend.Data;
using VidizmoBackend.Models;
using Microsoft.EntityFrameworkCore;
using VidizmoBackend.DTOs;

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
            // delete video tags associated with the video
            var videoTags = await _context.VideoTags
                .Where(vt => vt.VideoId == video.VideoId)
                .ToListAsync();
            _context.VideoTags.RemoveRange(videoTags);
            // delete the video itself
            _context.Videos.Remove(video);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<MetadataResDto> GetMetadataByIdAsync(int videoId)
        {
            var video = await _context.Videos
                .Where(v => v.VideoId == videoId)
                .Select(v => new MetadataResDto
                {
                    VideoId = v.VideoId,
                    Title = v.Title,
                    Description = v.Description,
                    UploadDate = v.UploadedAt,
                    FileSize = v.FileSize,
                    Format = v.FileFormat,
                    UploadedBy = v.UploadedByUser.Firstname + " " + v.UploadedByUser.Lastname,
                    Tags = v.VideoTags.Select(t => t.Tag.Title).ToList()
                })
                .FirstOrDefaultAsync();

            return video ?? new MetadataResDto();
        }

        public async Task<bool> EditVideoMetadataAsync(MetadataReqDto metadataReqDto, int videoId)
        {
            var video = await _context.Videos.FindAsync(videoId);
            if (video == null)
            {
                return false;
            }

            video.Title = metadataReqDto.Title ?? video.Title;
            video.Description = metadataReqDto.Description ?? video.Description;

            // Update tags if provided
            if (metadataReqDto.Tags != null && metadataReqDto.Tags.Count != 0)
            {
                // Clear existing tags
                var existingTags = await _context.VideoTags
                    .Where(vt => vt.VideoId == videoId)
                    .ToListAsync();
                _context.VideoTags.RemoveRange(existingTags);

                // Add new tags
                foreach (var tagTitle in metadataReqDto.Tags)
                {
                    var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Title == tagTitle);
                    if (tag == null)
                    {
                        tag = new Tag { Title = tagTitle };
                        _context.Tags.Add(tag);
                        await _context.SaveChangesAsync();
                    }
                    _context.VideoTags.Add(new VideoTag { VideoId = videoId, TagId = tag.TagId });
                }
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAllVideosByUserIdAsync(int userId)
        {
            // Find all videos uploaded by the user
            var videos = await _context.Videos
                .Where(v => v.UploadedByUserId == userId)
                .ToListAsync();

            if (videos.Count == 0) return true; // No videos to delete

            // Remove all video tags associated with these videos
            var videoIds = videos.Select(v => v.VideoId).ToList();
            var videoTags = await _context.VideoTags
                .Where(vt => videoIds.Contains(vt.VideoId))
                .ToListAsync();
            _context.VideoTags.RemoveRange(videoTags);

            // Remove the videos themselves
            _context.Videos.RemoveRange(videos);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Video>> GetAllVideosByUserIdAsync(int userId)
        {
            return await _context.Videos
                .Where(v => v.UploadedByUserId == userId)
                .ToListAsync();
        }

        public async Task<List<GetAllVideosDto>?> GetAllVideos(int orgId)
        {
            var videoList = await _context.Videos
                .Where(v => v.OrganizationId == orgId)
                .Select(v => new GetAllVideosDto
                {
                    VideoId = v.VideoId,
                    Title = v.Title,
                    Description = v.Description,
                })
                .ToListAsync();

            return videoList;
        }
    }
}
