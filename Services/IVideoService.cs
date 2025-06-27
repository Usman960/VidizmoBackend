using VidizmoBackend.DTOs;

namespace VidizmoBackend.Services
{
    public interface IVideoService
    {
        Task<bool> UploadVideoAsync(int userId, int orgId, NotifyUploadDto dto);
        Task<Stream> DownloadVideoAsync(int videoId);
        Task<(Stream stream, string contentType, string fileName)> StreamVideoAsync(int videoId);
        Task<bool> DeleteVideoAsync(int videoId);
        Task<bool> DeleteAllVideosAsync(int userId);
        Task<MetadataResDto> GetMetadataByIdAsync(int videoId);
        Task<bool> EditVideoMetadataAsync(MetadataReqDto metadataReqDto, int videoId);
        Task<List<GetAllVideosDto>?> GetAllVideos(int orgId);
    }
}