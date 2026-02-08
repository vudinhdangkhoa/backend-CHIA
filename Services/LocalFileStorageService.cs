using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Interfaces.Services;

namespace server.Services
{
    public class LocalFileStorageService : IStorageService
    {
       private readonly IWebHostEnvironment _env;

        public LocalFileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        
        public async Task<string>UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            
            var uploadPath = Path.Combine(_env.WebRootPath, "uploadsPhotos");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var uniqueFileName= $"{Guid.NewGuid()}_{Path.GetExtension(fileName)}";
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using(var Stream =new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(Stream);
            }
    
            return $"/uploadsPhotos/{uniqueFileName}";
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var filePath = Path.Combine(_env.WebRootPath, fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            await Task.CompletedTask;
        }

    }
}