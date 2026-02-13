using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Core.Interfaces.Services
{
    public interface IMailServices
    {
        Task SendEmailAsync(string to, string subject, string body);
        
    }
}