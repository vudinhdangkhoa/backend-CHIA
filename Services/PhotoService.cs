using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Entities;
using server.Core.Interfaces.Services;
using server.Data;

namespace server.Services
{

    public interface IPhotoService
    {
        Task<Photo> UploadPhotoAsync(Guid userId, Stream fileStream, string fileName, string caption);
    }

    public class PhotoService : IPhotoService
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
                return null;
                throw new Exception("file is empty");

            }

            var photoUrl = await _storage.UploadFileAsync(fileStream, fileName, server.Core.Enum.ContentType.image);

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
    }
}