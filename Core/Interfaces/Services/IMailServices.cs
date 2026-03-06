using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server.Core.Interfaces.Services
{
    public interface IMailServices
    {
        // Method gửi email cơ bản
        Task SendEmailAsync(string to, string subject, string body);

        // Method gửi OTP
        Task SendOtpEmailAsync(string UserId,string to,  int expiryMinutes = 5);

        // Method cảm ơn thanh toán
        Task SendPaymentThankYouAsync(string to, string orderId, decimal amount);
    }
}