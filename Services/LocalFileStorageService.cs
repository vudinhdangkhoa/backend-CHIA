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
            
            if(contentType != server.Core.Enum.ContentType.image.ToString() && contentType != server.Core.Enum.ContentType.video.ToString())
            {
                throw new Exception("Unsupported content type");
            }
            if (fileStream == null || fileStream.Length == 0)
            {
                throw new Exception("File is empty");
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new Exception("File name is required");
            }

            string uploadPath ="";

            if(contentType == server.Core.Enum.ContentType.image.ToString())
            {
                var allowedImageExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(fileName).ToLower();
                if (!allowedImageExtensions.Contains(fileExtension))
                {
                    throw new Exception("Invalid image file extension");
                }
                uploadPath = Path.Combine(_env.WebRootPath, $"uploads{contentType}");
            }
            else if(contentType == server.Core.Enum.ContentType.video.ToString())
            {
                var allowedVideoExtensions = new List<string> { ".mp4", ".avi", ".mov", ".wmv" };
                var fileExtension = Path.GetExtension(fileName).ToLower();
                if (!allowedVideoExtensions.Contains(fileExtension))
                {
                    throw new Exception("Invalid video file extension");
                }
                uploadPath = Path.Combine(_env.WebRootPath, $"uploads{contentType}");
            }
            

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
    
            return $"/uploads{contentType}/{uniqueFileName}";
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