using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            try {
                var result = await _authService.AuthenticateAsync(dto);
                if (result == null)
                    return Unauthorized(new { error = "Invalid email or password." });

                return Ok(new { Token = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred: " + ex.Message });
            }
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(SignUpRequestDto dto)
        {
            try
            {
                var result = await _authService.SignUpAsync(dto);
                if (!result)
                    return StatusCode(500, new { error = "Something went wrong while creating the user." });
                
                return Ok(new { message = "User signed up successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred: " + ex.Message });
            }
        }
    }
}
