using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Core.Interfaces.Services;
using server.Data;
using server.Services;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IJwtService _jwtServices;
        private readonly AppDbContext _dbContext;

        private readonly IAuthServices _authService;

        public AuthController(AppDbContext dbContext, IAuthServices authService, IJwtService jwtServices)
        {
            _dbContext = dbContext;
            _authService = authService;
            _jwtServices = jwtServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTO.LoginRequest request)
        {
            //

            if (request == null)
            {
                return BadRequest(new { Message = "Invalid request" });
            }

            if(string.IsNullOrEmpty(request.Mail) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Mail hoặc mật khẩu bị trống" });
            }

            // Call the AuthService to handle login
           
           try
           {
             var AuthResponse = await _authService.LoginAsync(request.Mail, request.Password, request.DeviceInfo);
             return Ok(new { AuthResponse, Message = "Đăng nhập thành công" });
           }
           catch (System.Exception ex)
           {
            return BadRequest(new { Message = "Đăng nhập thất bại: " + ex.Message });
            throw;
           }

           
           
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTO.RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { Message = "Invalid request" });
            }
            Console.WriteLine("request: " + request.Mail + " - " + request.Password + " - " + request.Username+ "-" + request.DeviceInfo.DeviceName+ "-" + request.DeviceInfo.Platform+ "-" + request.DeviceInfo.FcmToken);
            if (string.IsNullOrEmpty(request.Mail) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.Username))
            {
                return BadRequest(new { Message = "Mail, mật khẩu và tên người dùng là bắt buộc" });
            }

            // Call the AuthService to handle registration
           try
           {
            Console.WriteLine("call services");
            Console.WriteLine("DeviceInfo: " + (request.DeviceInfo != null ? $"DeviceName={request.DeviceInfo.DeviceName}, Platform={request.DeviceInfo.Platform}, FcmToken={request.DeviceInfo.FcmToken}" : "null"));
            Console.WriteLine("_authService null? " + (_authService == null));
            var authResponse = await _authService.RegisterAsync(request.Mail, request.Password, request.Username,request.DeviceInfo);
                return Ok(new{  Message = "Đăng ký thành công" });
           }
           catch (System.Exception ex)
           {
            Console.WriteLine("Error in AuthController.Register: " + ex.Message);
            return BadRequest(new { Message = $"Đăng ký thất bại: {ex.Message}" });
            throw;
           }

            
        }




    }
}