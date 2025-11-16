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

    public async Task<LoginResponseDto> Login(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null)
            throw new UnauthorizedAccessException("Kullanıcı bulunamadı.");

        var result = _hasher.VerifyHashedPassword(null, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Geçersiz şifre.");

        var token = _tokenService.GenerateToken(user.Username, user.Role);

        return new LoginResponseDto
        {
            Username = user.Username,
            Role = user.Role,
            Token = token,
            Expiration = DateTime.UtcNow.AddHours(1)
        };
    }
}
