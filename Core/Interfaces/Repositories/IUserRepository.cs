using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Entities;
using server.DTO;

namespace server.Core.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Hàm hỗ trợ lấy list token của 1 user
        Task<List<string>> GetFcmTokensAsync(Guid userId);

        // Hàm xử lý khi user đăng nhập (Lưu token mới)
        Task SaveOrUpdateDeviceAsync(Guid userId, DeviceInfo deviceInfo);

        // Hàm xử lý khi user đăng xuất (Xóa token để không push nhầm nữa)
        Task RemoveDeviceAsync(string fcmToken);
    }
}