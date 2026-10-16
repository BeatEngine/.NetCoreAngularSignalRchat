using AngularDemoChatSignalR.Server.Controllers;
using AngularDemoChatSignalR.Server.Database.Entities;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace AngularDemoChatSignalR.Server.HubsWebsocket
{
    /// <summary>
    /// Mapping='/sigR/notis'
    /// </summary>
    public class RealtimeNotificationHub : Hub
    {
        private ILogger<ChatController> _logger;
        private RealtimeNotificationHubService realtimeNotificationHubService;
        public RealtimeNotificationHub(ILogger<ChatController> logger, RealtimeNotificationHubService realtimeNotificationHubService)
        {
            this._logger = logger;
            this.realtimeNotificationHubService = realtimeNotificationHubService;
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            RealtimeNotificationHubService.userIdByConnectionId.Remove(this.Context.ConnectionId);
            return;
        }

        /// <summary>
        /// Register clients by userId, so notifications will be sent only to the group of users
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HubMethodName("registerConnectionUser")]
        public async Task registerConnectionUser(Int64 userId)
        {
            RealtimeNotificationHubService.userIdByConnectionId[this.Context.ConnectionId] = userId;
            _logger.LogInformation($"Registered connection {this.Context.ConnectionId} for user {userId}", ConsoleColor.Blue);
            return;
        }

        
    }
}
