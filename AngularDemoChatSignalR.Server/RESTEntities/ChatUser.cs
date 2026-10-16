using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository;
using Microsoft.VisualBasic;
using System.Numerics;

namespace AngularDemoChatSignalR.Server.Entities
{
    /// <summary>
    /// Same structur in Frontend: angulardemochatsignalr.client\src\app\chat\ChatUser.ts
    /// </summary>
    public class ChatUser
    {
        public Int64 userId { get; set; }
        public String userName { get; set; }
        public String previewMessage { get; set; } = "";

        public ChatUser()
        {
            userId = 0;
        }


        public ChatUser(Int64 userId, String userName, String previewMessage = "") 
        { 
            this.userId = userId;
            this.userName = userName;
            this.previewMessage = previewMessage;
        }

        public static ICollection<ChatUser> FromJoinUserChatList(ICollection<JoinUserChat> joinUserChats, Int64 currentUserId)
        {
            ICollection<ChatUser> results = new List<ChatUser>();

            HashSet<Int64> ids = new HashSet<Int64>();

            foreach(JoinUserChat chat in joinUserChats)
            {
                //Only add unique authors or receivers as chats, that doesn't have the current userId. The order is descending, so the previewmessage the newest msg.
                if (chat.AuthorId != currentUserId)
                {
                    if (ids.Add(chat.AuthorId))
                    {
                        results.Add(new ChatUser(chat.AuthorId, chat?.AuthorName ?? "", chat?.Message ?? ""));
                    }
                }
                if (chat?.ReceiverId != currentUserId)
                {
                    if (ids.Add(chat.ReceiverId))
                    {
                        results.Add(new ChatUser(chat.ReceiverId, chat?.ReceiverName ?? "", chat?.Message ?? ""));
                    }
                }
            }

            return results;
        }


        public static ICollection<ChatUser> FromJoinUserList(ICollection<User> users)
        {
            ICollection<ChatUser> results = new List<ChatUser>();
            foreach (User user in users)
            {
                results.Add(new ChatUser(user.UserId, user?.UserName ?? "NO-NAME-ERROR"));
            }
            return results;
        }

    }
}
