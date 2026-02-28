using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Entities;

namespace server.Core.Interfaces.Repositories
{
    public interface IFriendshipRepository
    {
        // Lấy danh sách UserID đang là bạn bè (Status = Accepted)
        Task<List<Guid>> GetFriendIdsAsync(Guid userId);

        // Kiểm tra xem 2 người có phải bạn không
        Task<bool> IsFriendAsync(Guid userId1, Guid userId2);

        // Lấy danh sách lời mời kết bạn đang chờ (Status = Pending)
        Task<List<Guid>> GetPendingFriendRequestIdsAsync(Guid userId);

        // Lấy danh sách lời mời kết bạn đã gửi (Status = Pending)
        Task<List<Guid>> GetSentFriendRequestIdsAsync(Guid userId);

        // Gửi lời mời kết bạn
        Task SendFriendRequestAsync(Guid senderId, Guid receiverId);

        //Accept lời mời kết bạn
        Task AcceptFriendRequestAsync(Guid userId, Guid friendId);

        // Từ chối lời mời kết bạn
        Task DeclineFriendRequestAsync(Guid userId, Guid friendId);

        // Hủy kết bạn
        Task UnfriendAsync(Guid userId, Guid friendId);
    
    }
}