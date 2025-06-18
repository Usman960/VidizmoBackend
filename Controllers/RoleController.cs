using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/role")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to create roles
                var permissionDto = new PermissionDto
                {
                    Action = "create",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to create roles.");
                }
                var result = await _roleService.CreateRoleAsync(userId, dto);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while creating the role.");
                }

                return Ok("Role created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("assign-user/{userId}/{orgId}/{roleId}")]
        public async Task<IActionResult> AssignRoleToUser(int userId, int orgId, int roleId)
        {
            try
            {
                int assignedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to assign roles
                var permissionDto = new PermissionDto
                {
                    Action = "assign",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(assignedByUserId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to assign roles.");
                }
                var result = await _roleService.AssignRoleToUserAsync(userId, orgId, roleId, assignedByUserId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while assigning the role.");
                }

                return Ok("Role assigned successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("assign-group/{groupId}/{orgId}/{roleId}")]
        public async Task<IActionResult> AssignRoleToGroup(int groupId, int orgId, int roleId)
        {
            try
            {
                int assignedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to assign roles to groups
                var permissionDto = new PermissionDto
                {
                    Action = "assign",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(assignedByUserId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to assign roles to groups.");
                }
                var result = await _roleService.AssignRoleToGroupAsync(groupId, orgId, roleId, assignedByUserId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while assigning the role to the group.");
                }

                return Ok("Role assigned to group successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPut("{roleId}")]
        public async Task<IActionResult> EditRole(int roleId, RoleDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to edit roles
                var permissionDto = new PermissionDto
                {
                    Action = "edit",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to edit roles.");
                }
                var result = await _roleService.EditRoleAsync(userId, roleId, dto);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while editing the role.");
                }

                return Ok("Role edited successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpDelete("{roleId}")]
        public async Task<IActionResult> DeleteRole(int roleId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to delete roles
                var permissionDto = new PermissionDto
                {
                    Action = "delete",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to delete roles.");
                }
                var result = await _roleService.DeleteRoleAsync(roleId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while deleting the role.");
                }

                return Ok("Role deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("revoke/{userOgGpRoleId}")]
        public async Task<IActionResult> RevokeRole(int userOgGpRoleId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to revoke roles
                var permissionDto = new PermissionDto
                {
                    Action = "revoke",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "You do not have permission to revoke roles.");
                }
                var result = await _roleService.RevokeRoleAsync(userId, userOgGpRoleId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while revoking the role.");
                }

                return Ok("Role revoked successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
        
    }
}