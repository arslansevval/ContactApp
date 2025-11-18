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
            var response = await _authService.Login(dto.Username, dto.Password);

            return Ok(response);
        }
    }
}
