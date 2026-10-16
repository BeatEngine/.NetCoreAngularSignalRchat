using System.Numerics;

namespace AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository
{
    public class JoinUserChat
    {
        public Int64 ChatId { get; set; }
        public Int64? MessageId { get; set; }
        public Int64 AuthorId { get; set; }
        public Int64 ReceiverId { get; set; }
        public String? AuthorName { get; set; }
        public String? ReceiverName { get; set; }
        public DateTime? Created { get; set; }

        public DateTime GetCreatedSafeOrdering()
        {
            if (this.Created != null)
            {
                return (DateTime)this.Created;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public String? Message { get; set; }
    }
}
