using Microsoft.Extensions.Hosting;
using System.Numerics;

namespace AngularDemoChatSignalR.Server.Database.Entities
{
    public class User
    {

        public Int64 UserId { get; set; }

        public DateTime Created { get; set; }
        public String? UserName { get; set; }

        public String? PassHash { get; set; }

        public ICollection<UserChat>? AuthorUserChats { get; set; }
        public ICollection<UserChat>? ReceiverUserChats { get; set; }

        public User(string UserName)
        {
            this.UserName = UserName;
            this.Created = DateTime.Now;
        }

        public User()
        {

        }
    }
}
