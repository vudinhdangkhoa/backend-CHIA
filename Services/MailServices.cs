using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Interfaces.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace server.Services
{
    public class MailServices : IMailServices
    {

        private readonly IConfiguration _emailConfig;
        private readonly ILogger<MailServices> _logger;
        private static readonly Dictionary<string, (string otp, DateTime expiry)> _otpStorage = new();

        public MailServices(IConfiguration emailConfig, ILogger<MailServices> logger)
        {
            _emailConfig = emailConfig;
            _logger = logger;
        }

        public string GenerateOTP(string userKey,int time) // userKey có thể là email hoặc userId
        {
            var random = new Random();
            var otp = random.Next(100000, 999999).ToString();
            var expiry = DateTime.Now.AddMinutes(time);

            // Lưu OTP theo từng user
            _otpStorage[userKey] = (otp, expiry);

            return otp;
        }

        public bool ValidateOTP(string userKey, string otp)
        {
            if (_otpStorage.TryGetValue(userKey, out var storedOtp))
            {
                if (storedOtp.otp == otp && storedOtp.expiry > DateTime.Now)
                {
                    _otpStorage.Remove(userKey); // Xóa OTP sau khi xác thực thành công
                    return true;
                }
            }
            return false;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig["EmailConfiguration:SenderName"], _emailConfig["EmailConfiguration:SenderEmail"]));
            emailMessage.To.Add(MailboxAddress.Parse(to));
            emailMessage.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = body };
            emailMessage.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfig["EmailConfiguration:SmtpServer"], int.Parse(_emailConfig["EmailConfiguration:SmtpPort"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailConfig["EmailConfiguration:Username"], _emailConfig["EmailConfiguration:Password"]);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}");
                throw;
            }
        }

        public async Task SendOtpEmailAsync(string userId,string to, int expiryMinutes = 5)
        {
            string otpCode = GenerateOTP(userId, expiryMinutes);
            var subject = "Mã xác thực OTP của bạn";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Xác thực tài khoản</h2>
                <p>Mã OTP của bạn là:</p>
                <h1 style='color: #4CAF50; font-size: 32px; letter-spacing: 5px;'>{otpCode}</h1>
                <p>Mã này sẽ hết hạn sau <strong>{expiryMinutes} phút</strong>.</p>
                <p style='color: #666; font-size: 12px;'>
                    Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.
                </p>
            </body>
            </html>";

            await SendEmailAsync(to, subject, body);
        }

        public async Task SendPaymentThankYouAsync(string to, string orderId, decimal amount)
        {
            var subject = "Cảm ơn bạn đã thanh toán";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Thanh toán thành công! 🎉</h2>
                <p>Cảm ơn bạn đã hoàn tất thanh toán.</p>
                <div style='background-color: #f5f5f5; padding: 15px; border-radius: 5px;'>
                    <p><strong>Mã đơn hàng:</strong> {orderId}</p>
                    <p><strong>Số tiền:</strong> {amount:N0} VND</p>
                </div>
                <p>Chúng tôi đánh giá cao sự tin tưởng của bạn!</p>
            </body>
            </html>";

            await SendEmailAsync(to, subject, body);
        }
    }
}