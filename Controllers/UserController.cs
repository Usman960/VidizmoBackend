using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.Services;
using VidizmoBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VidizmoBackend.Helpers;
using VidizmoBackend.Models;
using VidizmoBackend.Filters;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly AuditLogService _auditLogService;
        public UserController(IUserService userService, IRoleService roleService, AuditLogService auditLogService)
        {
            _userService = userService;
            _roleService = roleService;
            _auditLogService = auditLogService;
        }

        [HttpPost("org")]
        public async Task<IActionResult> AddUserToOrganization(AddUserToOrgDto dto)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            int orgId = int.Parse(User.FindFirst("OrganizationId")?.Value);
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
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to add users to organization." });

                var result = await _userService.AddUserToOrganizationAsync(orgId, dto.Email);
                if (!result)
                    return StatusCode(500, "Failed to add user to organization.");

                return Ok(new { message = "User added to organization successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("group/{groupId}/{userId}")]
        [EnforceTenant(
            new [] {"groupId", "userId"},
            new [] {typeof(Group), typeof(User)}
        )]
        public async Task<IActionResult> AddUserToGroup(int groupId, int userId)
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
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to add users to groups." });

                var result = await _userService.AddUserToGroupAsync(groupId, userId, currentUserId);
                if (!result)
                    return StatusCode(500, "Failed to add user to group.");

                return Ok(new { message = "User added to group successfully." });
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

        [HttpDelete("group/{groupId}/{userId}")]
        [EnforceTenant(
            new [] {"groupId", "userId"},
            new [] {typeof(Group), typeof(User)}
        )]
        public async Task<IActionResult> RemoveUserFromGroup(int groupId, int userId)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                // Check if the current user has permission to remove users from groups
                var permissionDto = new PermissionDto
                {
                    Action = "delete_gp",
                    Entity = "user"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(currentUserId, permissionDto);
                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to remove users from groups." });

                var result = await _userService.RemoveUserFromGroupAsync(groupId, userId);
                if (!result)
                    return StatusCode(500, "Failed to remove user from group.");

                return Ok(new { message = "User removed from group successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersInOrganization()
        {
            try
            {
                int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int orgId = int.Parse(User.FindFirst("OrganizationId")?.Value);
                // Check if the current user has permission to remove users from groups
                var permissionDto = new PermissionDto
                {
                    Action = "view",
                    Entity = "user"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(currentUserId, permissionDto);
                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view users." });

                var users = await _userService.GetUsersInOrganization(orgId);

                return Ok(users);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("group/{groupId}")]
        [EnforceTenant(
            new [] {"groupId"},
            new [] {typeof(Group)}
        )]
        public async Task<IActionResult> GetUsersNotInGroup(int groupId)
        {
            try
            {
                var list = await _userService.GetUsersNotInGroup(groupId);

                return Ok(new { usersNotInGroup = list });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}