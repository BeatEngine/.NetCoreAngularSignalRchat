using System.Numerics;

namespace AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository
{
    public class JoinUserChatMessage
    {
        public Int64 ChatId { get; set; }
        public Int64? MessageId { get; set; }
        public Int64 AuthorId { get; set; }
        public Int64 ReceiverId { get; set; }
        public DateTime? Created { get; set; }

        public DateTime? Updated { get; set; }
        public String? Message { get; set; }
    }
}
