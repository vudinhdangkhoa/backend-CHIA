using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Common;
using server.DTO;

namespace server.Core.Interfaces.Services
{
    public interface IAuthServices
    {
       Task<AuthResponse> LoginAsync(string phoneNumber, string password, DeviceInfo deviceInfo = null);
        Task<AuthResponse> RegisterAsync(string phoneNumber, string password,string username, DeviceInfo deviceInfo = null);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string refreshToken);
    }
}