using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Entities;
using server.Core.Interfaces.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;

namespace server.Services
{
    public class JwtServices : IJwtService
    {
        private readonly IConfiguration _config;
        public JwtServices(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.Username)
          };

            var token = new JwtSecurityToken(

              issuer: _config["Jwt:Issuer"],
              audience: _config["Jwt:Audience"],
              claims: claims,
              expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryInMinutes"])),
              signingCredentials: credentials

            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);

        }

    }
}