using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace server.Services.Hubs
{
    [Authorize]
    public class FeedHub:Hub
    {
        
        public  override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier; // Lấy userId từ Claims
           
            await base.OnConnectedAsync();
        }

    }
}