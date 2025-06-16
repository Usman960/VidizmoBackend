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

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole(CreateRoleDto dto)
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

        [HttpPost("assign/{userId}/{orgId}/{roleId}")]
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
    }
}