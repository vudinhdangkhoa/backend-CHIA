using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Entities;

namespace server.Core.Interfaces.Repositories
{
    public interface IPhotoRepository 
    {
        // Lấy danh sách ảnh từ danh sách bạn bè, sắp xếp mới nhất
        Task<IEnumerable<Photo>> GetFeedForUserAsync(List<Guid> friendIds, int page, int pageSize);

        //Lấy ảnh theo UserId
        Task<List<server.Services.PhotoGroup>> GetPhotosByUserIdAsync(Guid userId);
        // Upload ảnh
         Task<Photo> UploadPhotoAsync(Guid userId, Stream fileStream, string fileName, string caption);
        
    }
}