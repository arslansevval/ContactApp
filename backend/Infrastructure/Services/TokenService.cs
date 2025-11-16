using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ContactApp.Core.Interfaces;

namespace ContactApp.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GenerateToken(string username, string role)
        {
            if (string.IsNullOrWhiteSpace(username)) 
                throw new ArgumentException("Username cannot be empty.", nameof(username));
            if (string.IsNullOrWhiteSpace(role)) 
                throw new ArgumentException("Role cannot be empty.", nameof(role));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var keyString = _config["Jwt:Key"] ?? throw new Exception("JWT Key config not found.");
            if (Encoding.UTF8.GetBytes(keyString).Length < 32)
                throw new Exception("JWT Secret key must be at least 256 bits (32 bytes).");

            var issuer = _config["Jwt:Issuer"] ?? throw new Exception("JWT Issuer config not found.");
            var audience = _config["Jwt:Audience"] ?? throw new Exception("JWT Audience config not found.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
