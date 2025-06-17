using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/group")]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private readonly GroupService _groupService;
        private readonly RoleService _roleService;

        public GroupController(GroupService groupService, RoleService roleService)
        {
            _groupService = groupService;
            _roleService = roleService;
        }

        [HttpPost("{orgId}")]
        public async Task<IActionResult> CreateGroup(int orgId, [FromBody] string groupName)
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
                var result = await _groupService.CreateGroupAsync(orgId, userId, groupName);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while creating the group.");
                }

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
                
                var result = await _groupService.DeleteGroupAsync(groupId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while deleting the group.");
                }

                return Ok("Group deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}