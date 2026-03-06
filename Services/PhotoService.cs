using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Core.Entities;
using server.Core.Interfaces.Repositories;
using server.Core.Interfaces.Services;
using server.Data;
using Microsoft.AspNetCore.SignalR;
using server.Services.Hubs;


namespace server.Services
{



    public class PhotoService : IPhotoRepository
    {

        private readonly AppDbContext db;
        private readonly IStorageService _storage;
        // private readonly INotificationService _noti; // Để làm thông báo
        private readonly IHubContext<FeedHub> _hubContext;
        private readonly IFriendshipRepository friendshipRepository;

        private readonly INotificationService notificationService;

        public PhotoService(AppDbContext context, IStorageService storage, IHubContext<FeedHub> hubContext, IFriendshipRepository friendRepository)
        {
            db = context;
            _storage = storage;
            _hubContext = hubContext;
            friendshipRepository = friendRepository;
        }

        public async Task<Photo> UploadPhotoAsync(Guid userId, Stream fileStream, string fileName, string caption)
        {

            var checkUserExist = await db.Users.FindAsync(userId); //TODO: check user exist in database

            if (checkUserExist == null) { return null; throw new Exception("user not found"); }

            if (fileStream == null || fileStream.Length == 0)
            {
                throw new Exception("file is empty");

            }

            var photoUrl = await _storage.UploadFileAsync(fileStream, fileName, server.Core.Enum.ContentType.image);

            if (photoUrl == null)
            {

                throw new Exception("upload file failed");
            }

            var Photo = new Photo
            {
                Id = Guid.NewGuid(),
                SenderId = userId,
                ImageUrl = photoUrl,
                Caption = caption,
                CreatedAt = DateTime.UtcNow
            };

            await db.Photos.AddAsync(Photo);
            await db.SaveChangesAsync();

            //gửi thông báo đến bạn bè 
            
            // Lấy list friendIds của userId
            var friendIds = await friendshipRepository.GetFriendIdsAsync(userId);
            // Gửi notification qua SignalR và FCM
            await notificationService.SendPhotoNotificationAsync(userId, checkUserExist.Username, friendIds, new
            {
                ImageUrl = Photo.ImageUrl,
                Caption = Photo.Caption,
                createdAt= Photo.CreatedAt
            });

            return Photo;
        }

        public async Task<List<PhotoGroup>> GetPhotosByUserIdAsync(Guid userId)
        {
            var photos = await db.Photos
                .Where(p => p.SenderId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return photos
                .GroupBy(p => p.CreatedAt.Date)
                .OrderByDescending(g => g.Key)
                .Select(g => new PhotoGroup
                {
                    Date = g.Key,
                    Photos = g.ToList()
                })
                .ToList();
        }



        public async Task<IEnumerable<PhotoDto>> GetFeedForUserAsync(List<Guid> friendIds, int page, int pageSize)
        {

            return await db.Photos
            .AsNoTracking() // Tối ưu hiệu năng
            .Where(p => friendIds.Contains(p.SenderId))
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PhotoDto
            {
                Id = p.Id,
                ImageUrl = p.ImageUrl,
                Caption = p.Caption,
                SenderName = p.Sender.Username, // Join tự động qua EF
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();
        }
    }
    // DTO class
    public class PhotoGroup
    {
        public DateTime Date { get; set; }
        public List<Photo> Photos { get; set; }
    }

    public class PhotoDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; }
        public string Caption { get; set; }
        public string SenderName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}