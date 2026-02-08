using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Core.Interfaces.Services
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
        Task DeleteFileAsync(string fileUrl);
    }
}