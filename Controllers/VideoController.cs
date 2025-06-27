using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.Services;
using VidizmoBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VidizmoBackend.Models;
using System.Text.Json;
using VidizmoBackend.Helpers;
using VidizmoBackend.Filters;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/video")]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly IRoleService _roleService;
        private readonly ITokenService _tokenService;
        private readonly AuditLogService _auditLogService;
        private readonly AzureBlobService _azureBlobService;
        public VideoController(IVideoService videoService, IRoleService roleService, ITokenService tokenService, AuditLogService auditLogService, AzureBlobService azureBlobService)
        {
            _videoService = videoService;
            _roleService = roleService;
            _tokenService = tokenService;
            _auditLogService = auditLogService;
            _azureBlobService = azureBlobService;
        }

        [HttpPost("generate-upload-url")]
        public async Task<IActionResult> GenerateUploadUrl([FromBody] UploadInitDto dto)
        {
            try
            {
                int? scopedTokenId = User.HasClaim(c => c.Type == "ScopedTokenId")
                    ? int.Parse(User.FindFirstValue("ScopedTokenId")!)
                    : null;

                int? userId = scopedTokenId == null
                    ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                    : null;

                // check if the user is authorized to upload videos
                var permissionDto = new PermissionDto
                {
                    Action = "upload",
                    Entity = "video"
                };

                bool hasPermission = scopedTokenId.HasValue
                    ? await _tokenService.TokenHasPermissionAsync(scopedTokenId.Value, permissionDto)
                    : await _roleService.UserHasPermissionAsync(userId!.Value, permissionDto);

                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to upload videos." });
                
                string blobName;
                string sasUrl = _azureBlobService.GenerateUploadSasUrl(dto.OriginalFileName, out blobName);

                return Ok(new { uploadUrl = sasUrl, blobName });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }            
        }

        [HttpPost("notify-upload")]
        public async Task<IActionResult> NotifyUpload([FromBody] NotifyUploadDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int orgId = int.Parse(User.FindFirst("OrganizationId")?.Value);

                var saved = await _videoService.UploadVideoAsync(userId, orgId, dto);
                if (!saved)
                    return StatusCode(500, "Failed to save video metadata.");

                
                return Ok(new { message = "Metadata saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to save metadata: " + ex.Message });
            }
        }

        [HttpGet("download/{videoId}")]
        [EnforceTenant(
            new [] {"videoId"},
            new [] {typeof(Video)}
        )]
        public async Task<IActionResult> DownloadVideo(int videoId)
        {
            try
            {
                int? scopedTokenId = User.HasClaim(c => c.Type == "ScopedTokenId")
                    ? int.Parse(User.FindFirstValue("ScopedTokenId")!)
                    : null;

                int? userId = scopedTokenId == null
                    ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                    : null;

                var permissionDto = new PermissionDto
                {
                    Action = "download",
                    Entity = "video"
                };
                bool hasPermission = scopedTokenId.HasValue
                    ? await _tokenService.TokenHasPermissionAsync(scopedTokenId.Value, permissionDto)
                    : await _roleService.UserHasPermissionAsync(userId!.Value, permissionDto);

                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to download videos." });

                var stream = await _videoService.DownloadVideoAsync(videoId);
                if (stream == null)
                    return NotFound("Video not found.");

                return File(stream, "application/octet-stream", $"video_{videoId}.mp4");
            }
            catch (FileNotFoundException)
            {
                return NotFound("Video not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("play/{videoId}")]
        [EnforceTenant(
            new [] {"videoId"},
            new [] {typeof(Video)}
        )]
        public async Task<IActionResult> PlayVideo(int videoId)
        {
            try
            {
                int? scopedTokenId = User.HasClaim(c => c.Type == "ScopedTokenId")
                    ? int.Parse(User.FindFirstValue("ScopedTokenId")!)
                    : null;

                int? userId = scopedTokenId == null
                    ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                    : null;

                var permissionDto = new PermissionDto
                {
                    Action = "play",
                    Entity = "video"
                };
                bool hasPermission = scopedTokenId.HasValue
                    ? await _tokenService.TokenHasPermissionAsync(scopedTokenId.Value, permissionDto)
                    : await _roleService.UserHasPermissionAsync(userId!.Value, permissionDto);

                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to play videos." });

                var (stream, contentType, fileName) = await _videoService.StreamVideoAsync(videoId);

                return File(stream, contentType, enableRangeProcessing: true);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Video not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Unexpected error: " + ex.Message);
            }
        }

        [HttpDelete("delete/{videoId}")]
        [EnforceTenant(
            new [] {"videoId"},
            new [] {typeof(Video)}
        )]
        public async Task<IActionResult> DeleteVideo(int videoId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var permissionDto = new PermissionDto
                {
                    Action = "delete",
                    Entity = "video"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to delete videos." });

                var deleted = await _videoService.DeleteVideoAsync(videoId);
                if (!deleted)
                    return NotFound(new { message = "Video not found or could not be deleted." });

                return Ok(new { message = "Video deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpGet("metadata/{videoId}")]
        [EnforceTenant(
            new [] {"videoId"},
            new [] {typeof(Video)}
        )]
        public async Task<IActionResult> GetVideoMetadata(int videoId)
        {
            try
            {
                int? scopedTokenId = User.HasClaim(c => c.Type == "ScopedTokenId")
                    ? int.Parse(User.FindFirstValue("ScopedTokenId")!)
                    : null;

                int? userId = scopedTokenId == null
                    ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                    : null;

                var permissionDto = new PermissionDto
                {
                    Action = "view",
                    Entity = "metadata"
                };
                bool hasPermission = scopedTokenId.HasValue
                    ? await _tokenService.TokenHasPermissionAsync(scopedTokenId.Value, permissionDto)
                    : await _roleService.UserHasPermissionAsync(userId!.Value, permissionDto);

                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view metadata." });

                var metadata = await _videoService.GetMetadataByIdAsync(videoId);

                return Ok(metadata);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Video metadata not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVideos()
        {
            try
            {
                int? scopedTokenId = User.HasClaim(c => c.Type == "ScopedTokenId")
                    ? int.Parse(User.FindFirstValue("ScopedTokenId")!)
                    : null;

                int? userId = scopedTokenId == null
                    ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                    : null;

                int orgId = int.Parse(User.FindFirst("OrganizationId")?.Value);

                var videoList = await _videoService.GetAllVideos(orgId);

                if (videoList == null || videoList.Count() == 0) return StatusCode(404, new { error = "No videos found in this tenant" });
                return Ok(new { VideoList = videoList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPut("metadata/{videoId}")]
        [EnforceTenant(
            new[] {"videoId"},
            new[] {typeof(Video)}
        )]
        public async Task<IActionResult> EditVideoMetadata(int videoId, MetadataReqDto metadataReqDto)
        {
            try
            {
                int? scopedTokenId = User.HasClaim(c => c.Type == "ScopedTokenId")
                    ? int.Parse(User.FindFirstValue("ScopedTokenId")!)
                    : null;

                int? userId = scopedTokenId == null
                    ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
                    : null;

                var permissionDto = new PermissionDto
                {
                    Action = "edit",
                    Entity = "metadata"
                };
                bool hasPermission = scopedTokenId.HasValue
                    ? await _tokenService.TokenHasPermissionAsync(scopedTokenId.Value, permissionDto)
                    : await _roleService.UserHasPermissionAsync(userId!.Value, permissionDto);

                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to edit metadata." });

                var updated = await _videoService.EditVideoMetadataAsync(metadataReqDto, videoId);

                return Ok(new { message = "Video metadata updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest("Failed to update video metadata: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest("Invalid request: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}
