using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.Services;
using VidizmoBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VidizmoBackend.Helpers;
using VidizmoBackend.Models;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly RoleService _roleService;
        private readonly AuditLogService _auditLogService;
        public UserController(UserService userService, RoleService roleService, AuditLogService auditLogService)
        {
            _userService = userService;
            _roleService = roleService;
            _auditLogService = auditLogService;
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

                var payload = AuditLogHelper.BuildPayload(routeData: new { userId, orgId });

                var log = new AuditLog
                {
                    Action = "add_org",
                    Entity = "user",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = currentUserId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

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

                var payload = AuditLogHelper.BuildPayload(new { groupId }, userIds);

                var log = new AuditLog
                {
                    Action = "add_gp",
                    Entity = "user",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = currentUserId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

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

        [HttpDelete("group/{groupId}")]
        public async Task<IActionResult> RemoveUserFromGroup(int groupId, [FromBody] List<int> userIds)
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
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to remove users from groups.");

                var result = await _userService.RemoveUsersFromGroupAsync(groupId, userIds);
                if (!result)
                    return StatusCode(500, "Failed to remove user from group.");

                var payload = AuditLogHelper.BuildPayload(new { groupId }, userIds);

                var log = new AuditLog
                {
                    Action = "delete_gp",
                    Entity = "user",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = currentUserId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok("User removed from group successfully.");
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

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetUsersWithRoles(int orgId)
        {
            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                // Check if the current user has permission to remove users from groups
                var permissionDto = new PermissionDto
                {
                    Action = "view",
                    Entity = "user"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(currentUserId, permissionDto);
                if (!hasPermission)
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to view users.");

                var users = await _userService.GetUsersWithRolesAsync(orgId);

                var payload = AuditLogHelper.BuildPayload(new { orgId });

                var log = new AuditLog
                {
                    Action = "view",
                    Entity = "user",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = currentUserId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

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
    }
}