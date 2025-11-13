using ContactApp.Core.Interfaces;

namespace ContactApp.Application.Services
{
    public class AuthService
    {
        private readonly ITokenService _tokenService;

        public AuthService(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public string Login(string username, string password)
        {
            // TODO: Gerçek user doğrulaması (veritabanından kontrol)
            if (username == "admin" && password == "1234")
            {
                return _tokenService.GenerateToken(username, "Admin");
            }

            throw new UnauthorizedAccessException("Geçersiz kullanıcı adı veya şifre");
        }
    }
}
