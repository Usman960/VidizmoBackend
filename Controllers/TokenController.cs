using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VidizmoBackend.Helpers;
using VidizmoBackend.Models;
using VidizmoBackend.Filters;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/token")]
    [Authorize]
    public class TokencController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IRoleService _roleService;
        private readonly AuditLogService _auditLogService;
        public TokencController(ITokenService tokenService, IRoleService roleService, AuditLogService auditLogService)
        {
            _tokenService = tokenService;
            _roleService = roleService;
            _auditLogService = auditLogService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateScopedToken(TokenDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int orgId = int.Parse(User.FindFirst("OrganizationId")?.Value);
                // Check if the user has permission to delete roles
                var permissionDto = new PermissionDto
                {
                    Action = "generate",
                    Entity = "scoped_token"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to generate token." });
                }
                var token = await _tokenService.GenerateScopedTokenAsync(userId, orgId, dto);
                var payload = AuditLogHelper.BuildPayload(new { orgId }, dto);

                var log = new AuditLog
                {
                    Action = "generate",
                    Entity = "scoped_token",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);
                var content = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(token));
                var fileName = $"scoped-token-{DateTime.UtcNow:yyyyMMdd-HHmmss}.txt";
                return File(content, "text/plain", fileName); ;
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("revoke/{tokenId}")]
        [EnforceTenant(
            new [] {"tokenId"},
            new [] {typeof(ScopedToken)}
        )]
        public async Task<IActionResult> RevokeToken(int tokenId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to revoke tokens
                var permissionDto = new PermissionDto
                {
                    Action = "revoke",
                    Entity = "scoped_token"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to revoke tokens." });
                }
                var result = await _tokenService.RevokeTokenAsync(tokenId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while revoking the token.");
                }
                var payload = AuditLogHelper.BuildPayload(routeData: new { tokenId });

                var log = new AuditLog
                {
                    Action = "revoke",
                    Entity = "scoped_token",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);
                return Ok(new { message = "Token revoked successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpDelete("{tokenId}")]
        [EnforceTenant(
            new [] {"tokenId"},
            new [] {typeof(ScopedToken)}
        )]
        public async Task<IActionResult> DeleteToken(int tokenId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to delete tokens
                var permissionDto = new PermissionDto
                {
                    Action = "delete",
                    Entity = "scoped_token"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to delete tokens.");
                }
                var result = await _tokenService.DeleteTokenAsync(tokenId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while deleting the token.");
                }
                var payload = AuditLogHelper.BuildPayload(routeData: new { tokenId });

                var log = new AuditLog
                {
                    Action = "delete",
                    Entity = "scoped_token",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);
                return Ok(new { message = "Token deleted successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyScopedTokens()
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _tokenService.GetTokensByUserId(userId);

                return Ok(new { tokens = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

    }
}

