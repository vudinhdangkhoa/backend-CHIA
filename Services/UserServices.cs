using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Core.Entities;
using server.Core.Interfaces.Repositories;
using server.Data;
using server.DTO;

namespace server.Services
{
    public class UserServices : IUserRepository
    {

        private readonly AppDbContext _context;

        public UserServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> FindAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
        {
            return await _context.Users.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
                _context.Users.Update(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task SaveOrUpdateDeviceAsync(Guid userId, DeviceInfo deviceInfo)
        {
            Console.WriteLine("_context null? " + (_context == null));
            Console.WriteLine("deviceInfo null? " + (deviceInfo == null));
            // Tìm device cũ theo FCM token hoặc device identifier
            var existingDevice = await _context.UserDevices
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

                await _context.UserDevices.AddAsync(newDevice);
            }
        }

        // Hàm hỗ trợ lấy list token của 1 user
        public async Task<List<string>> GetFcmTokensAsync(Guid userId)
        {
            return await _context.UserDevices
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .Select(d => d.FcmToken)
                .ToListAsync();
        }

        // Hàm xử lý khi user đăng xuất (Xóa token để không push nhầm nữa)
        public async Task RemoveDeviceAsync(string fcmToken)
        {
            var device = await _context.UserDevices.FirstOrDefaultAsync(d => d.FcmToken == fcmToken);
            if (device != null)
            {
                device.IsDeleted = true;
                _context.UserDevices.Update(device);
                await _context.SaveChangesAsync();
            }
        }
    }
}