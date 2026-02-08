using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using server.Core.Entities;

namespace server.Data.configurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(f => f.Id);

            // Cấu hình mối quan hệ với Requester (Người gửi lời mời)
            builder.HasOne(f => f.Requester)
                .WithMany(u => u.SentFriendRequests)
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict); // QUAN TRỌNG: Restrict để tránh lỗi Cycle/Multiple Cascade Paths

            // Cấu hình mối quan hệ với Receiver (Người nhận lời mời)
            builder.HasOne(f => f.Receiver)
                .WithMany(u => u.ReceivedFriendRequests)
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho phép xóa User kéo theo xóa Friendship tự động ở cả 2 đầu
        }
    }
}