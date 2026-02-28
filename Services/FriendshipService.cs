using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Core.Entities;
using server.Core.Interfaces.Repositories;
using server.Data;
using server.Enum;

namespace server.Services
{
    public class FriendshipService : IFriendshipRepository
    {
        private readonly AppDbContext db;
        public FriendshipService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<List<Guid>> GetFriendIdsAsync(Guid userId)
        {
            var friendIds = await db.Friendships
                .Where(t => (t.RequesterId == userId || t.ReceiverId == userId) && t.Status == FriendshipStatus.Accepted)
                .Select(t => t.RequesterId == userId ? t.ReceiverId : t.RequesterId)
                .ToListAsync();

            return friendIds;
        }

        public async Task<List<Guid>> GetPendingFriendRequestIdsAsync(Guid userId)
        {
            var pendingFriendRequestIds = await db.Friendships
                .Where(t => t.ReceiverId == userId && t.Status == FriendshipStatus.Pending)
                .Select(t => t.RequesterId)
                .ToListAsync();

            return pendingFriendRequestIds;
        }

        public async Task<List<Guid>> GetSentFriendRequestIdsAsync(Guid userId)
        {
            var sentFriendRequestIds = await db.Friendships
                .Where(t => t.RequesterId == userId && t.Status == FriendshipStatus.Pending)
                .Select(t => t.ReceiverId)
                .ToListAsync();

            return sentFriendRequestIds;
        }


        public async Task<bool> IsFriendAsync(Guid userId1, Guid userId2)
        {
            return await db.Friendships.FirstOrDefaultAsync(t => t.RequesterId == userId1 && t.ReceiverId == userId2 && t.Status == FriendshipStatus.Accepted) != null
                || await db.Friendships.FirstOrDefaultAsync(t => t.RequesterId == userId2 && t.ReceiverId == userId1 && t.Status == FriendshipStatus.Accepted) != null;
        }

        public async Task SendFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            if (await IsFriendAsync(senderId, receiverId) == true)
            {
                throw new Exception("đã là bạn bè");
            }

            var request = new Friendship
            {
                Id = Guid.NewGuid(),
                RequesterId = senderId,
                ReceiverId = receiverId,
                Status = FriendshipStatus.Pending, // Sử dụng Enum trạng thái
                CreatedAt = DateTime.UtcNow
            };

            db.Friendships.Add(request);
            await db.SaveChangesAsync();

        }

        public async Task AcceptFriendRequestAsync(Guid userId, Guid friendId)
        {

            if(await IsFriendAsync(userId, friendId) == true)
            {
                throw new Exception("đã là bạn bè");
            }

            var request = await db.Friendships.FirstOrDefaultAsync(t => t.RequesterId == friendId && t.ReceiverId == userId && t.Status == FriendshipStatus.Pending);
            if (request == null)
            {
                throw new Exception("Không tìm thấy lời mời kết bạn");
            }

            request.Status = FriendshipStatus.Accepted; // Cập nhật trạng thái thành Accepted
            db.Friendships.Update(request);
            await db.SaveChangesAsync();
        }
        
        public async Task DeclineFriendRequestAsync(Guid userId, Guid friendId)
        {
            var request = await db.Friendships.FirstOrDefaultAsync(t => t.RequesterId == friendId && t.ReceiverId == userId && t.Status == FriendshipStatus.Pending);
            if (request == null)
            {
                throw new Exception("Không tìm thấy lời mời kết bạn");
            }

            request.Status = FriendshipStatus.Decline; // Cập nhật trạng thái thành Declined
            db.Friendships.Update(request);
            await db.SaveChangesAsync();
        }

        public async Task UnfriendAsync(Guid userId, Guid friendId)
        {
            if(await IsFriendAsync(userId, friendId) == false)
            {
                throw new Exception("chưa là bạn bè");
            }

            var friendship = await db.Friendships.FirstOrDefaultAsync(t =>
                (t.RequesterId == userId && t.ReceiverId == friendId) ||
                (t.RequesterId == friendId && t.ReceiverId == userId));

            db.Friendships.Remove(friendship);
            await db.SaveChangesAsync();
        }
    }
}