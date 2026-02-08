using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Core.Interfaces.Services
{
    public interface INotificationService
    {
        // Hàm này sẽ phải query lấy TẤT CẢ device của userId đó để gửi
        Task SendToUserAsync(Guid userId, string title, string body, object data);
    }
}