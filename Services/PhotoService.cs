using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using server.Core.Entities;
using server.Core.Interfaces.Repositories;
using server.Core.Interfaces.Services;
using server.Data;

namespace server.Services
{



    public class PhotoService : IPhotoRepository
    {

        private readonly AppDbContext db;
        private readonly IStorageService _storage;
        // private readonly INotificationService _noti; // Để làm thông báo

        public PhotoService(AppDbContext context, IStorageService storage)
        {
            db = context;
            _storage = storage;
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

            //gửi thông báo đến bạn bè ở đây

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



        public async Task<IEnumerable<Photo>> GetFeedForUserAsync(List<Guid> friendIds, int page, int pageSize)
        {
            return await db.Photos
                .Where(p => friendIds.Contains(p.SenderId))
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
    // DTO class
    public class PhotoGroup
    {
        public DateTime Date { get; set; }
        public List<Photo> Photos { get; set; }
    }
}