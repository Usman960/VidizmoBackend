using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/token")]
    public class TokencController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly RoleService _roleService;
        public TokencController(TokenService tokenService, RoleService roleService)
        {
            _tokenService = tokenService;
            _roleService = roleService;
        }
        
        [HttpPost("generate/{orgId}")]
        public async Task<IActionResult> GenerateScopedToken(int orgId, TokenDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to delete roles
                var permissionDto = new PermissionDto
                {
                    Action = "generate",
                    Entity = "scoped_token"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to delete roles.");
                }
                var token = await _tokenService.GenerateScopedTokenAsync(userId, orgId, dto);
                return Ok(new { Token = token });
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
    }
}

