using Microsoft.AspNetCore.Mvc;
using ContactApp.Application.Services;
using ContactApp.Application.DTOs.Auth;

namespace ContactApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var user = await _authService.Login(dto.Username, dto.Password);

            if (user == null)
            {
                // Başarısız login
                return Unauthorized(new 
                {
                    IsOk = false,
                    Message = "Invalid username or password"
                });
            }

            // Başarılı login
            var response = new LoginResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role,
                Token = user.Token,
                Expiration = user.Expiration
            };

            return Ok(new 
            {
                IsOk = true,
                Message = "Login successful",
                Data = response
            });
        }
    }
}
