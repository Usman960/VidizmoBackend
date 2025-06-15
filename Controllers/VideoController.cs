using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.Services;
using VidizmoBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/video")]
    [Authorize]
    public class VideoController : ControllerBase
    {
        private readonly VideoService _videoService;

        public VideoController(VideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo([FromForm] IFormFile file, [FromForm] AddVideoReqDto dto)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            if (dto == null)
                return BadRequest("Video details are required.");

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var saved = await _videoService.UploadVideoAsync(file, dto, userId);
                if (!saved)
                    return StatusCode(500, "Failed to upload video.");

                return Ok("Video uploaded successfully.");
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
        public async Task<IActionResult> PlayVideo(int videoId)
        {
            try
            {
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
        public async Task<IActionResult> DeleteVideo(int videoId)
        {
            try
            {
                var deleted = await _videoService.DeleteVideoAsync(videoId);
                if (!deleted)
                    return NotFound("Video not found or could not be deleted.");

                return Ok("Video deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("metadata/{videoId}")]
        public async Task<IActionResult> GetVideoMetadata(int videoId)
        {
            try
            {
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

        [HttpPut("metadata/{videoId}")]
        public async Task<IActionResult> EditVideoMetadata(int videoId, MetadataReqDto metadataReqDto)
        {
            try
            {
                var updated = await _videoService.EditVideoMetadataAsync(metadataReqDto, videoId);

                return Ok("Video metadata updated successfully.");
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
