using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.DTO
{
    public class LoginRequest
    {
        public string Mail { get; set; }
        public string Password { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
    }
}