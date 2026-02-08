using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using server.Common;


namespace server.Core.Entities
{
    public class UserDevice : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }

        // Token dùng để push noti (Đây là cái bạn quan tâm)
        [Required]
        public string FcmToken { get; set; }

        // (Tùy chọn) Để biết token này của máy nào, tránh lưu trùng
        public string? DeviceId { get; set; } // Unique Device ID (ví dụ: Android ID, iOS Vendor ID)
        public string? Platform { get; set; } // "iOS", "Android"

        public DateTime LastActiveAt { get; set; } // Để sau này viết Job xóa token rác (lâu ko dùng)
    }
}