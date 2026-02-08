using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.DTO
{
   public class DeviceInfo
    {
        public string DeviceName { get; set; }  // "iPhone 14 Pro"
        public string FcmToken { get; set; }     // Firebase token
        public string Platform { get; set; }     // "iOS", "Android", "Web"
    }
}