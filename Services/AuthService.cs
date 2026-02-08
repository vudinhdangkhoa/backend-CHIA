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

namespace server.Services
{
    public class AuthService : IAuthServices
    {
        private readonly AppDbContext db;
        private readonly IJwtService _jwt;

        public AuthService(AppDbContext db, IJwtService jwt)
        {
            this.db = db;
            _jwt = jwt;
        }

        //  Thêm tham số deviceInfo
        public async Task<AuthResponse> LoginAsync(string phoneNumber, string password, DeviceInfo deviceInfo = null)
        {
            var user = await db.Users.FirstOrDefaultAsync(t => t.PhoneNumber == phoneNumber);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            if (user.PasswordHash != password)
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
                await SaveOrUpdateDeviceAsync(user.Id, deviceInfo);
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
        public async Task<AuthResponse> RegisterAsync(string phoneNumber, string password,string username, DeviceInfo deviceInfo = null)
        {
            var existingUser = await db.Users.FirstOrDefaultAsync(t => t.PhoneNumber == phoneNumber);
            if (existingUser != null)
            {
                throw new Exception("Phone number already registered");
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber,
                PasswordHash = password,
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
                await SaveOrUpdateDeviceAsync(newUser.Id, deviceInfo);
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
                ExpiresAt = DateTime.UtcNow.AddDays(30),
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

        //  Helper method để lưu/update device
        private async Task SaveOrUpdateDeviceAsync(Guid userId, DeviceInfo deviceInfo)
        {
            // Tìm device cũ theo FCM token hoặc device identifier
            var existingDevice = await db.UserDevices
                .FirstOrDefaultAsync(d => 
                    d.UserId == userId && 
                    d.FcmToken == deviceInfo.FcmToken);

            if (existingDevice != null)
            {
                // Cập nhật device cũ
                existingDevice.DeviceId = deviceInfo.DeviceName;
                existingDevice.Platform = deviceInfo.Platform;
                existingDevice.LastActiveAt = DateTime.UtcNow;
                
            }
            else
            {
                // Tạo device mới
                var newDevice = new UserDevice
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    DeviceId = deviceInfo.DeviceName,
                    FcmToken = deviceInfo.FcmToken,
                    Platform = deviceInfo.Platform,
                    LastActiveAt = DateTime.UtcNow,
                   
                    CreatedAt = DateTime.UtcNow
                };

                await db.UserDevices.AddAsync(newDevice);
            }
        }

        public async Task<AuthResponse> RegisterAsync(string phoneNumber, string password)
        {
            var existingUser = await db.Users.FirstOrDefaultAsync(t => t.PhoneNumber == phoneNumber);
            if (existingUser != null)
            {
                throw new Exception("Phone number already registered");
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                PhoneNumber = phoneNumber,
                PasswordHash = password
            };

            await db.Users.AddAsync(newUser);
            await db.SaveChangesAsync();

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
            await db.SaveChangesAsync();

            return new AuthResponse{
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                UserId = newUser.Id
            };
        }

    }
}