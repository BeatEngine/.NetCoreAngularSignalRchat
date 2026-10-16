using AngularDemoChatSignalR.Server.Controllers;
using Microsoft.AspNetCore.SignalR;

namespace AngularDemoChatSignalR.Server.HubsWebsocket
{
    public class RealtimeNotificationHubService
    {
        private IHubContext<RealtimeNotificationHub> realtimeNotificationHub;
        private readonly ILogger<ChatController> _logger;

        public static Dictionary<string, Int64> userIdByConnectionId = new Dictionary<string, long>();

        public RealtimeNotificationHubService(ILogger<ChatController> logger, IHubContext<RealtimeNotificationHub> realtimeNotificationHub) 
        {
            this._logger = logger;
            this.realtimeNotificationHub = realtimeNotificationHub;
        }


        /// <summary>
        /// SendRestUpdateNotification to all clients with userId (Privat for frontend client usable for server)
        /// </summary>
        /// <param name="restEndpointPath"> The API path the client can update e.g. '/api/Chat/...'</param>
        /// <returns></returns>
        public async Task SendRestUpdateNotification(Int64 userId, string restEndpointPath)
        {

            foreach (var element in userIdByConnectionId)
            {
                if (element.Value == userId)
                {
                    _ = realtimeNotificationHub.Clients.Client(element.Key).SendAsync("ChangedNotificationREST" /* Same name in angulardemochatsignalr.client\src\app\chat\chat.component.ts: realtimeConnection.on */
                        , restEndpointPath);
                    _logger.LogInformation($"Sent REST notification connection {restEndpointPath} to user {userId}");
                }
            }
        }
    }
}
