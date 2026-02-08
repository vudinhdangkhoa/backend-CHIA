using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Core.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? FcmToken { get; }
    }
}