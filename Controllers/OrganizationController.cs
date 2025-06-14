using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VidizmoBackend.Controllers {
    [ApiController]
    [Route("api/organization")]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly OrganizationService _organizationService;

        public OrganizationController(OrganizationService organizationService)
        {
            _organizationService = organizationService;
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