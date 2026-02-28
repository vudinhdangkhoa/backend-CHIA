using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Core.Common;
using server.Core.Entities;
using server.Core.Interfaces.Services;
using server.Data;
using server.DTO;
using BCrypt.Net;
using server.Core.Interfaces.Repositories;
namespace server.Services
{
    public class AuthService : IAuthServices
    {
        private readonly AppDbContext db;
        private readonly IJwtService _jwt;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        public AuthService(AppDbContext db, IJwtService jwt, IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            this.db = db;
            _jwt = jwt;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        //  Thêm tham số deviceInfo
        public async Task<AuthResponse> LoginAsync(string mail, string password, DeviceInfo deviceInfo = null)
        {
            var user = await db.Users.FirstOrDefaultAsync(t => t.Mail == mail);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (!_passwordHasher.Verify(password, user.PasswordHash))
            {
                throw new Exception("Invalid password");
            }

            var accessToken = _jwt.GenerateAccessToken(user);
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = _jwt.GenerateRefreshToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await db.RefreshTokens.AddAsync(refreshToken);

            //  Lưu hoặc cập nhật thiết bị
            if (deviceInfo != null)
            {
                await _userRepository.SaveOrUpdateDeviceAsync(user.Id, deviceInfo);
            }

            await db.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                UserId = user.Id
            };
        }

        //  Thêm tham số deviceInfo
        public async Task<AuthResponse> RegisterAsync(string mail, string password,string username, DeviceInfo deviceInfo = null)
        {
            var existingUser = await db.Users.FirstOrDefaultAsync(t => t.Mail == mail);
            if (existingUser != null)
            {
                throw new Exception("Mail already registered");
            }
            var passwordHash = _passwordHasher.Hash(password);
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Mail = mail,
                PasswordHash = passwordHash,
                Username = username
            };

            await db.Users.AddAsync(newUser);

            var accessToken = _jwt.GenerateAccessToken(newUser);
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = newUser.Id,
                Token = _jwt.GenerateRefreshToken(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await db.RefreshTokens.AddAsync(refreshToken);

            //  Lưu thiết bị
            if (deviceInfo != null)
            {
                await _userRepository.SaveOrUpdateDeviceAsync(newUser.Id, deviceInfo);
            }

            await db.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                UserId = newUser.Id
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await db.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || !storedToken.IsActive)
                throw new UnauthorizedAccessException("Invalid refresh token");

            var newAccessToken = _jwt.GenerateAccessToken(storedToken.User);
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = storedToken.UserId,
                Token = _jwt.GenerateRefreshToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            storedToken.RevokedAt = DateTime.UtcNow;
         
            await db.RefreshTokens.AddAsync(newRefreshToken);
            await db.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                UserId = storedToken.UserId
            };
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var token = await db.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token != null && token.IsActive)
            {
                token.RevokedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
        }

        

        

    }
}