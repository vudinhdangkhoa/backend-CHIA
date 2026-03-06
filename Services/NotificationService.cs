using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using server.Core.Interfaces.Services;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.SignalR;
using server.Services.Hubs;
using server.Core.Interfaces.Repositories;

namespace server.Services
{
    public class NotificationService:INotificationService
    {
        private readonly IHubContext<FeedHub> _hubContext;
        private readonly IUserRepository _userRepository;

        public NotificationService(IHubContext<FeedHub> hubContext, IUserRepository userRepository)
        {
            _hubContext = hubContext;
            _userRepository = userRepository;
        }

        public async Task SendPhotoNotificationAsync(Guid senderId, string senderName, List<Guid> friendIds, object photoData)
        {
            //gui notification qua SignalR
            var FriendIdStr= friendIds.Select(id => id.ToString()).ToList();
            await _hubContext.Clients.Users(FriendIdStr).SendAsync("ReceivePhotoNotification", new
            {
                SenderId = senderId,
                photodata = photoData,
            });

            //gui notification qua FCM
            foreach(var friendId in friendIds)
            {
                // lay token device cua friendId
                var token = await _userRepository.GetFcmTokensAsync(friendId);
                if(token != null)
                {
                    var message = new MulticastMessage()
                    {
                        
                        Tokens=token,
                        Notification= new Notification()
                        {
                            Title="CHIA",
                            Body=$"{senderName} vừa chia sẻ một bức ảnh mới!"
                            
                        },
                        Data = new Dictionary<string, string>() { { "type", "new_photo" } }

                    };
                    await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                }

            }
        }
    }
}