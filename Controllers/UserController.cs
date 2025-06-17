using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.Services;
using VidizmoBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;

        public UserController(UserService userService, RoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpPost("org/{userId}/{orgId}")]
        public async Task<IActionResult> AddUserToOrganization(int userId, int orgId)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                // Check if the current user has permission to add users to organizations
                var permissionDto = new PermissionDto
                {
                    Action = "add_org",
                    Entity = "user"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(currentUserId, permissionDto);
                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to add users to organizations.");

                var result = await _userService.AddUserToOrganizationAsync(userId, orgId);
                if (!result)
                    return StatusCode(500, "Failed to add user to organization.");

                return Ok("User added to organization successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("group/{groupId}")]
        public async Task<IActionResult> AddUserToGroup(int groupId, [FromBody] List<int> userIds)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                // Check if the current user has permission to add users to groups
                var permissionDto = new PermissionDto
                {
                    Action = "add_gp",
                    Entity = "user"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(currentUserId, permissionDto);
                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to add users to groups.");

                var result = await _userService.AddUsersToGroupAsync(groupId, userIds, currentUserId);
                if (!result)
                    return StatusCode(500, "Failed to add user to group.");

                return Ok("User added to group successfully.");
            }
            catch (ArgumentException ex)
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