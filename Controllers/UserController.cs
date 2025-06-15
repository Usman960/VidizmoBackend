using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.Services;
using VidizmoBackend.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("{userId}/{orgId}")]
        public async Task<IActionResult> AddUserToOrganization(int userId, int orgId)
        {
            try
            {
                var result = await _userService.AddUserToOrganizationAsync(userId, orgId);
                if (!result)
                    return StatusCode(500, "Failed to add user to organization.");

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
    }
}