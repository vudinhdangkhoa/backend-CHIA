using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.DTO
{
    public class RegisterRequest:LoginRequest
    {
        public  string Username { get; set; }
    }
}