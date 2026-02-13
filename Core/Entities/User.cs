using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Common;

namespace server.Core.Entities
{
    public class User : BaseEntity
    {
        public string Mail { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public string Bio { get; set; }
        public string PasswordHash { get; set; } // Lưu hash của mật khẩu, không lưu mật khẩu gốc
                                                 
        public ICollection<UserDevice> Devices { get; set; }=new List<UserDevice>(); // Token này thay đổi mỗi lần đăng nhập thiết bị mới

        public ICollection<RefreshToken> RefreshTokens { get; set; }=new List<RefreshToken>();// Token làm mới để lấy access token mới

        // Navigation Properties (Quan hệ EF Core)
        public ICollection<Photo> Photos { get; set; }

        // Quan hệ bạn bè 2 chiều khá phức tạp, ta sẽ định nghĩa qua Friendship
        public ICollection<Friendship> SentFriendRequests { get; set; }
        public ICollection<Friendship> ReceivedFriendRequests { get; set; }
    }
}