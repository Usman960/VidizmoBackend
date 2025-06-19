using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VidizmoBackend.Helpers;
using VidizmoBackend.Models;

namespace VidizmoBackend.Controllers {
    [ApiController]
    [Route("api/organization")]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationService _organizationService;
        private readonly AuditLogService _auditLogService;
        public OrganizationController(OrganizationService organizationService, AuditLogService auditLogService)
        {
            _organizationService = organizationService;
            _auditLogService = auditLogService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganization(CreateOrgReqDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _organizationService.CreateOrganizationAsync(userId, dto);
                if (!result)
                {
                    return StatusCode(500, "An error occurred while creating the organization.");
                }
                var payload = AuditLogHelper.BuildPayload(bodyData: dto);

                var log = new AuditLog
                {
                    Action = "create",
                    Entity = "org",
                    Timestamp = DateTime.UtcNow,
                    PerformedById = userId,
                    Payload = payload
                };
                _ = _auditLogService.SendLogAsync(log);

                return Ok("Organization created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteOrganization(int id) { }
    }

}