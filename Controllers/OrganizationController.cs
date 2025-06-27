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
        private readonly IOrganizationService _organizationService;
        private readonly AuditLogService _auditLogService;
        private readonly JwtTokenGenerator _tokenGenerator;
        public OrganizationController(IOrganizationService organizationService, AuditLogService auditLogService, JwtTokenGenerator tokenGenerator)
        {
            _organizationService = organizationService;
            _auditLogService = auditLogService;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganization(CreateOrgReqDto dto)
        {
            try
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var updatedUser = await _organizationService.CreateOrganizationAsync(userId, dto);

                var (token, _) = _tokenGenerator.GenerateToken(updatedUser);

                return Ok(new { message = "Organization created successfully.", Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganizationById()
        {
            try
            {
                int orgId = int.Parse(User.FindFirst("OrganizationId")?.Value);
                var name = await _organizationService.GetOrgNameById(orgId);
                return Ok(new { Name = name });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

}