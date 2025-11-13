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
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            var token = _authService.Login(dto.Username, dto.Password);

            var response = new LoginResponseDto
            {
                Username = dto.Username,
                Role = "Admin",
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1)
            };

            return Ok(response);
        }
    }
}
