using VidizmoBackend.DTOs;
using VidizmoBackend.Models;
using VidizmoBackend.Repositories;

namespace VidizmoBackend.Services
{
    public class VideoService
    {
        private readonly IVideoRepository _videoRepository;
        private readonly AzureBlobService _blobService;
        private readonly IOrgRepository _orgRepository;
        public VideoService(IVideoRepository videoRepository, AzureBlobService blobService, IOrgRepository orgRepository)
        {
            _videoRepository = videoRepository;
            _blobService = blobService;
            _orgRepository = orgRepository;
        }

        public async Task<bool> UploadVideoAsync(IFormFile file, AddVideoReqDto dto, int? userId, int? scopedTokenId, int orgId)
        {
            var blobUrl = await _blobService.UploadFileAsync(file);
            if (string.IsNullOrEmpty(blobUrl))
            {
                throw new InvalidOperationException("Failed to upload video to blob storage.");
            }

            var video = new Video
            {
                Filename = Path.GetFileNameWithoutExtension(file.FileName),
                FileSize = file.Length,
                Title = dto.Title,
                Description = dto.Description,
                UploadedByUserId = userId,
                ScopedTokenId = scopedTokenId,
                FilePath = blobUrl,
                OrganizationId = orgId,
                UploadedAt = DateTime.UtcNow,
                FileFormat = Path.GetExtension(file.FileName).TrimStart('.')
            };
            if (!await _videoRepository.UploadVideoAsync(video))
            {
                throw new InvalidOperationException("Failed to save video metadata to the database.");
            }
            return true;
        }

        public async Task<Stream> DownloadVideoAsync(int videoId)
        {
            var video = await _videoRepository.GetVideoByIdAsync(videoId);
            if (video == null)
            {
                throw new FileNotFoundException("Video not found.");
            }

            var blobName = Path.GetFileName(new Uri(video.FilePath).LocalPath);
            return await _blobService.DownloadFileAsync(blobName);
        }

        public async Task<(Stream stream, string contentType, string fileName)> StreamVideoAsync(int videoId)
        {
            var video = await _videoRepository.GetVideoByIdAsync(videoId);
            if (video == null)
                throw new FileNotFoundException();

            var blobName = Path.GetFileName(new Uri(video.FilePath).LocalPath);
            var (stream, contentType) = await _blobService.StreamFileAsync(blobName);

            return (stream, contentType, blobName);
        }

        public async Task<bool> DeleteVideoAsync(int videoId)
        {
            var video = await _videoRepository.GetVideoByIdAsync(videoId);
            if (video == null)
                throw new FileNotFoundException();

            var blobName = Path.GetFileName(new Uri(video.FilePath).LocalPath);
            // Delete from blob
            await _blobService.DeleteFileAsync(blobName);

            // Delete from DB
            await _videoRepository.DeleteVideoAsync(video);

            return true;
        }

        public async Task<bool> DeleteAllVideosAsync(int userId)
        {
            var videos = await _videoRepository.GetAllVideosByUserIdAsync(userId);
            if (videos == null) return true;

            foreach (var video in videos)
            {
                var blobName = Path.GetFileName(new Uri(video.FilePath).LocalPath);
                // Delete from blob
                await _blobService.DeleteFileAsync(blobName);
            }
            return await _videoRepository.DeleteAllVideosByUserIdAsync(userId);
        }

        public async Task<MetadataResDto> GetMetadataByIdAsync(int videoId)
        {
            var video = await _videoRepository.GetVideoByIdAsync(videoId);
            if (video == null)
            {
                throw new FileNotFoundException("Video not found.");
            }
            var metadata = await _videoRepository.GetMetadataByIdAsync(videoId);
            if (metadata == null)
            {
                throw new FileNotFoundException("Video metadata not found.");
            }
            return metadata;
        }

        public async Task<bool> EditVideoMetadataAsync(MetadataReqDto metadataReqDto, int videoId)
        {
            if (metadataReqDto == null || videoId <= 0)
            {
                throw new ArgumentException("Invalid metadata or video ID.");
            }
            var video = await _videoRepository.GetVideoByIdAsync(videoId);
            if (video == null)
            {
                throw new FileNotFoundException("Video not found.");
            }

            var updated = await _videoRepository.EditVideoMetadataAsync(metadataReqDto, videoId);
            if (!updated)
            {
                throw new InvalidOperationException("Failed to update video metadata.");
            }
            return true;
        }
    }
}
