using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Common;
using server.Enum;

namespace server.Core.Entities
{
    public class Friendship : BaseEntity
{
    public Guid RequesterId { get; set; } // Người gửi lời mời
    public User Requester { get; set; }

    public Guid ReceiverId { get; set; } // Người nhận lời mời
    public User Receiver { get; set; }

    public FriendshipStatus Status { get; set; }
}
}