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
    [Route("api/group")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly IRoleService _roleService;
        private readonly AuditLogService _auditLogService;

        public GroupController(IGroupService groupService, IRoleService roleService, AuditLogService auditLogService)
        {
            _groupService = groupService;
            _roleService = roleService;
            _auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(CreateGroupDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int orgId = int.Parse(User.FindFirst("OrganizationId")?.Value);
                // Check if the user has permission to create groups
                var permissionDto = new PermissionDto
                {
                    Action = "create",
                    Entity = "group"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to create groups." });
                }
                var result = await _groupService.CreateGroupAsync(orgId, userId, dto.GroupName);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while creating the group.");
                }

                return Ok(new {message = "Group created successfully."});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpDelete("{groupId}")]
        [EnforceTenant(
            new [] {"groupId"},
            new [] {typeof(Group)}
        )]
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
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to delete groups." });
                }

                await _roleService.DeleteAssignmentsByGroupId(groupId);
                var result = await _groupService.DeleteGroupAsync(groupId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while deleting the group.");
                }

                return Ok(new { message = "Group deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDetailedGroupList()
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                int orgId = int.Parse(User.FindFirst("OrganizationId")?.Value);
                var permissionDto = new PermissionDto
                {
                    Action = "view",
                    Entity = "group"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view groups." });
                }

                var list = await _groupService.GetGroupLists(orgId);

                return Ok(new {groupLists = list});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving groups: {ex.Message}");
            }
        }

    }
}