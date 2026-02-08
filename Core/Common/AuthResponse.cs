using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Core.Common
{
    public class AuthResponse
    {
         public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public Guid UserId { get; set; }
    }
}