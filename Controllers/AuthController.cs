using Microsoft.AspNetCore.Mvc;
using VidizmoBackend.DTOs;
using VidizmoBackend.Services;

namespace VidizmoBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            try {
                var result = await _authService.AuthenticateAsync(dto);
                if (result == null)
                    return Unauthorized("Invalid email or password.");

                return Ok(new { Token = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred: " + ex.Message);
            }
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp(SignUpRequestDto dto)
        {
            try
            {
                var result = await _authService.SignUpAsync(dto);
                if (!result)
                    return StatusCode(500, "Something went wrong while creating the user.");
                
                return Ok("User signed up successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
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
