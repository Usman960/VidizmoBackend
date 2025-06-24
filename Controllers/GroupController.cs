using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VidizmoBackend.Helpers;
using VidizmoBackend.Models;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/group")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly GroupService _groupService;
        private readonly RoleService _roleService;
        private readonly AuditLogService _auditLogService;

        public GroupController(GroupService groupService, RoleService roleService, AuditLogService auditLogService)
        {
            _groupService = groupService;
            _roleService = roleService;
            _auditLogService = auditLogService;
        }

        [HttpPost("{orgId}")]
        public async Task<IActionResult> CreateGroup(int orgId, CreateGroupDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Check if the user has permission to create groups
                var permissionDto = new PermissionDto
                {
                    Action = "create",
                    Entity = "group"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to create groups.");
                }
                var result = await _groupService.CreateGroupAsync(orgId, userId, dto.GroupName);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while creating the group.");
                }
                var payload = AuditLogHelper.BuildPayload(new { orgId }, dto);

                var log = new AuditLog
                {
                    Action = "create",
                    Entity = "group",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok("Group created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Check if the user has permission to delete groups
                var permissionDto = new PermissionDto
                {
                    Action = "delete",
                    Entity = "group"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to delete groups.");
                }

                await _roleService.DeleteAssignmentsByGroupId(groupId);
                var result = await _groupService.DeleteGroupAsync(groupId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while deleting the group.");
                }
                var payload = AuditLogHelper.BuildPayload(routeData: new { groupId });

                var log = new AuditLog
                {
                    Action = "delete",
                    Entity = "group",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok("Group deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetDetailedGroupList(int orgId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var permissionDto = new PermissionDto
                {
                    Action = "view",
                    Entity = "group"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to view groups.");
                }

                var list = await _groupService.GetGroupLists(orgId);
               
                var payload = AuditLogHelper.BuildPayload(routeData: new { orgId });

                var log = new AuditLog
                {
                    Action = "view",
                    Entity = "group",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(new {groupLists = list});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving groups: {ex.Message}");
            }
        }

    }
}