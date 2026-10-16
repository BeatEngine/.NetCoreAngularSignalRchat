using AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository;
using AngularDemoChatSignalR.Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularDemoChatSignalR.Test.RESTEntities
{
    [Collection("TestREST")]
    public class TestChatUser
    {
        private List<JoinUserChat> joinedUserChats;

        private Int64 currentUserId = 1;

        private void Init()
        {

            // A --> B without message
            JoinUserChat c1 = new JoinUserChat();
            c1.ChatId = 1;
            c1.Created = DateTime.Now;
            c1.AuthorId = currentUserId;
            c1.ReceiverId = 2;
            c1.AuthorName = $"User {c1.AuthorId}";
            c1.ReceiverName = $"User {c1.ReceiverId}";
            joinedUserChats.Add(c1);
            // B --> A with Message 
            JoinUserChat c2 = new JoinUserChat();
            c2.ChatId = 2;
            c2.Created = DateTime.Now;
            c2.AuthorId = 2;
            c2.ReceiverId = currentUserId;
            c2.AuthorName = $"User {c2.AuthorId}";
            c2.ReceiverName = $"User {c2.ReceiverId}";
            c2.MessageId = 101;
            c2.Message = "Hello A greetings B (B-->A)";
            joinedUserChats.Add(c2);
            // C --> A with Message 
            JoinUserChat c3 = new JoinUserChat();
            c3.ChatId = 2;
            c3.Created = DateTime.Now;
            c3.AuthorId = 3;
            c3.ReceiverId = currentUserId;
            c3.AuthorName = $"User {c3.AuthorId}";
            c3.ReceiverName = $"User {c3.ReceiverId}";
            c3.MessageId = 101;
            c3.Message = "Hello A greetings C (C-->A)";
            joinedUserChats.Add(c3);

        }

        public TestChatUser()
        {
            joinedUserChats = new List<JoinUserChat>();
            Init();
        }


        [Fact]
        public void FromJoinUserChatList()
        {

            ICollection<ChatUser> chatUsers = ChatUser.FromJoinUserChatList(joinedUserChats, currentUserId);

            Assert.True(chatUsers.Where(x => x.userId == currentUserId).Count() == 0, "Current user must not be contained!");

            Assert.True(chatUsers.Count == 2, "Count is wrong!");

            foreach (ChatUser chatUser in chatUsers)
            {
                if(chatUser.userName == "")
                {
                    Assert.Fail($"FromJoinUserChatList returns empty UserNames: chatUser.userName = '{chatUser.userName}'");
                }
            }

            Assert.True(joinedUserChats[2].Message.Equals(chatUsers.ToList()[1].previewMessage), "Preview messages are not equal. (First User must be skipped [2] == [2-1])");

        }

    }
}
