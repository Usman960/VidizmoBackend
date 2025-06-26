using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VidizmoBackend.Models;
using System.Text.Json;
using VidizmoBackend.Helpers;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/role")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly AuditLogService _auditLogService;
        public RoleController(IRoleService roleService, AuditLogService auditLogService)
        {
            _roleService = roleService;
            _auditLogService = auditLogService;
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
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to create roles." });
                }
                var result = await _roleService.CreateRoleAsync(userId, dto);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while creating the role.");
                }
                var payload = AuditLogHelper.BuildPayload(bodyData: dto);

                var log = new AuditLog
                {
                    Action = "create",
                    Entity = "role",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(new { message = "Role created successfully." });
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
                    Action = "assign_role",
                    Entity = "user"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(assignedByUserId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to assign individual roles." });
                }
                var result = await _roleService.AssignRoleToUserAsync(userId, orgId, roleId, assignedByUserId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while assigning the role.");
                }
                var payload = AuditLogHelper.BuildPayload(routeData: new { userId, orgId, roleId });
                var log = new AuditLog
                {
                    Action = "assign_role",
                    Entity = "user",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log); ;

                return Ok(new { message = "Role assigned successfully." });
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
                    Action = "assign_role",
                    Entity = "group"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(assignedByUserId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to assign roles to groups." });
                }
                var result = await _roleService.AssignRoleToGroupAsync(groupId, orgId, roleId, assignedByUserId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while assigning the role to the group.");
                }
                var payload = AuditLogHelper.BuildPayload(routeData: new { groupId, orgId, roleId });
                var log = new AuditLog
                {
                    Action = "assign_role",
                    Entity = "group",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = assignedByUserId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(new { message = "Role assigned to group successfully." });
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
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to edit roles." });
                }
                var result = await _roleService.EditRoleAsync(userId, roleId, dto);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while editing the role.");
                }

                var payload = AuditLogHelper.BuildPayload(new { roleId }, dto);

                var log = new AuditLog
                {
                    Action = "edit",
                    Entity = "role",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(new { message = "Role edited successfully." });
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
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to delete roles." });
                }
                var result = await _roleService.DeleteRoleAsync(roleId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while deleting the role.");
                }

                var payload = AuditLogHelper.BuildPayload(routeData: new { roleId });
                var log = new AuditLog
                {
                    Action = "delete",
                    Entity = "role",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(new { message = "Role deleted successfully." });
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
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to revoke roles." });
                }
                var result = await _roleService.RevokeRoleAsync(userId, userOgGpRoleId);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while revoking the role.");
                }
                var payload = AuditLogHelper.BuildPayload(routeData: new { userOgGpRoleId });
                var log = new AuditLog
                {
                    Action = "revoke",
                    Entity = "role",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(new { message = "Role revoked successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("{orgId}")]
        public async Task<IActionResult> GetAllRoles(int orgId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to revoke roles
                var permissionDto = new PermissionDto
                {
                    Action = "view",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view roles." });
                }
                var roles = await _roleService.GetAllRoles(orgId);

                var payload = AuditLogHelper.BuildPayload(routeData: new { orgId });
                var log = new AuditLog
                {
                    Action = "view",
                    Entity = "role",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("individual/{orgId}")]
        public async Task<IActionResult> GetIndividualRoles(int orgId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to revoke roles
                var permissionDto = new PermissionDto
                {
                    Action = "view",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view roles." });
                }
                var roles = await _roleService.GetIndividualRoles(orgId);

                var payload = AuditLogHelper.BuildPayload(routeData: new { orgId });
                var log = new AuditLog
                {
                    Action = "view",
                    Entity = "role",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet("group/{orgId}")]
        public async Task<IActionResult> GetGroupRoles(int orgId)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                // Check if the user has permission to revoke roles
                var permissionDto = new PermissionDto
                {
                    Action = "view",
                    Entity = "role"
                };
                var hasPermission = await _roleService.UserHasPermissionAsync(userId, permissionDto);
                if (!hasPermission)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new { message = "You do not have permission to view roles." });
                }
                var roles = await _roleService.GetGroupRoles(orgId);

                var payload = AuditLogHelper.BuildPayload(routeData: new { orgId });
                var log = new AuditLog
                {
                    Action = "view",
                    Entity = "role",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }
    }
}