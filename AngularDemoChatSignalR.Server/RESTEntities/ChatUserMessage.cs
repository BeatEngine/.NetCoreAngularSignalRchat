using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository;
using Microsoft.VisualBasic;
using System.Numerics;

namespace AngularDemoChatSignalR.Server.Entities
{
    /// <summary>
    /// Same structur in Frontend: angulardemochatsignalr.client\src\app\chat\ChatUserMessageType.ts
    /// </summary>
    public class ChatUserMessage
    {
        public DateTime created {  get; set; }
        /// <summary>
        /// Can be authorId or partnerId
        /// </summary>
        public Int64 userId { get; set; }
        public String message { get; set; }

        public ChatUserMessage()
        {
            
        }


        public ChatUserMessage(DateTime created, Int64 userId, String message) 
        { 
            this.created = created;
            this.userId = userId;
            this.message = message;
        }

        public static ICollection<ChatUserMessage> FromJoinUserChatMessagesList(ICollection<JoinUserChatMessage> joinUserChatMessages)
        {
            ICollection<ChatUserMessage> results = new List<ChatUserMessage>();
            foreach(JoinUserChatMessage chatMessage in joinUserChatMessages)
            {
                if (chatMessage != null && chatMessage?.AuthorId != null && chatMessage?.Message != null)
                {
                    results.Add(new ChatUserMessage(chatMessage?.Created ?? DateTime.MinValue, chatMessage.AuthorId, chatMessage.Message));
                }
            }
            return results;
        }


    }
}
