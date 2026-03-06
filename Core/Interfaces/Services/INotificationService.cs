using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Core.Interfaces.Services
{
    public interface INotificationService
    {
        // Hàm này sẽ phải query lấy TẤT CẢ device của list friendIds đó để gửi
        Task SendPhotoNotificationAsync(Guid senderId, string senderName, List<Guid> friendIds, object photoData);
    }
}