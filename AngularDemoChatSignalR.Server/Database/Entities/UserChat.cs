using System.Numerics;

namespace AngularDemoChatSignalR.Server.Database.Entities
{
    public class UserChat
    {
        public Int64 Id { get; set; }
        public Int64 AuthorId {  get; set; }
        public Int64 ReceiverId { get; set; }

        public User? Author { get; set; }
        public User? Receiver { get; set; }

        public ICollection<UserChatMessage>? UserChatMessages { get; set; }

        public String getAuthorName()
        {
            return Author?.UserName ?? "";
        }

        public String getReceiverName()
        {
            return Receiver?.UserName ?? "";
        }

        public UserChat()
        {

        }

        public UserChat(Int64 AuthorId, Int64 ReceiverId)
        {
            this.AuthorId = AuthorId;
            this.ReceiverId = ReceiverId;
        }
    }
}
