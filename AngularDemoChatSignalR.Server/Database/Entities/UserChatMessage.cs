using System.Numerics;

namespace AngularDemoChatSignalR.Server.Database.Entities
{
    public class UserChatMessage
    {

        public Int64 Id {  get; set; }
        public Int64 UserChatId { get; set; }

        public DateTime Created {  get; set; }
        public DateTime Updated { get; set; }
        public String? Message { get; set; }

        public UserChat? UserChat { get; set; }

        public UserChatMessage() { }

        public UserChatMessage(long userChatId, string? message)
        {
            UserChatId = userChatId;
            Created = DateTime.Now;
            Updated = Created;
            Message = message;
        }
    }
}
