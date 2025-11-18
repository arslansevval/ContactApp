using ContactApp.Application.DTOs.Auth;
using ContactApp.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<string> _hasher;

    public AuthService(IUserRepository userRepository, ITokenService tokenService)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _hasher = new PasswordHasher<string>();
    }

    public async Task<LoginResponseDto?> Login(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null)
            return null; // kullanıcı bulunamadı

        var result = _hasher.VerifyHashedPassword(user.Username, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
            return null; // şifre yanlış

        var token = _tokenService.GenerateToken(user.Username, user.Role);

        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role,
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(5) // Token süresi 5 dakika  
        };
    }
}
