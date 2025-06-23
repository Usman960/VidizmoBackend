using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.Services;
using VidizmoBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VidizmoBackend.Models;
using System.Text.Json;
using VidizmoBackend.Helpers;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/video")]
    public class VideoController : ControllerBase
    {
        private readonly VideoService _videoService;
        private readonly RoleService _roleService;
        private readonly TokenService _tokenService;
        private readonly AuditLogService _auditLogService;
        public VideoController(VideoService videoService, RoleService roleService, TokenService tokenService, AuditLogService auditLogService)
        {
            _videoService = videoService;
            _roleService = roleService;
            _tokenService = tokenService;
            _auditLogService = auditLogService;
        }

        [HttpPost("upload/{orgId}")]
        public async Task<IActionResult> UploadVideo([FromForm] IFormFile file, [FromForm] AddVideoReqDto dto, int orgId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            if (dto == null)
                return BadRequest("Video details are required.");

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
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to upload videos.");

                var saved = await _videoService.UploadVideoAsync(file, dto, userId, scopedTokenId, orgId);
                if (!saved)
                    return StatusCode(500, "Failed to upload video.");

                var payload = AuditLogHelper.BuildPayload(new { orgId }, dto);
                var log = new AuditLog
                {
                    Action = "upload",
                    Entity = "video",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    TokenId = scopedTokenId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);
                return Ok(new {message = "Video uploaded successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("download/{videoId}")]
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
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to download videos.");

                var stream = await _videoService.DownloadVideoAsync(videoId);
                if (stream == null)
                    return NotFound("Video not found.");

                var payload = AuditLogHelper.BuildPayload(routeData: new { videoId });
                var log = new AuditLog
                {
                    Action = "download",
                    Entity = "video",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    TokenId = scopedTokenId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);
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
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to play videos.");

                var (stream, contentType, fileName) = await _videoService.StreamVideoAsync(videoId);

                var payload = AuditLogHelper.BuildPayload(routeData: new { videoId });
                var log = new AuditLog
                {
                    Action = "play",
                    Entity = "video",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    TokenId = scopedTokenId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

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

                var payload = AuditLogHelper.BuildPayload(routeData: new { videoId });
                var log = new AuditLog
                {
                    Action = "delete",
                    Entity = "video",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(new { message = "Video deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpGet("metadata/{videoId}")]
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
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to view metadata.");

                var metadata = await _videoService.GetMetadataByIdAsync(videoId);

                var payload = AuditLogHelper.BuildPayload(routeData: new { videoId });
                var log = new AuditLog
                {
                    Action = "view",
                    Entity = "metadata",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    TokenId = scopedTokenId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

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

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetAllVideos(int orgId)
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
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to view videos.");

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
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to edit metadata.");

                var updated = await _videoService.EditVideoMetadataAsync(metadataReqDto, videoId);
                
                var payload = AuditLogHelper.BuildPayload(new { videoId }, metadataReqDto);
                var log = new AuditLog
                {
                    Action = "edit",
                    Entity = "metadata",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    TokenId = scopedTokenId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(new {message = "Video metadata updated successfully."});
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
