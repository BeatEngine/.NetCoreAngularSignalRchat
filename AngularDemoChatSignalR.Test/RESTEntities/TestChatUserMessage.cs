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
    public class TestChatUserMessage
    {
        private List<JoinUserChatMessage> joinedUserChatMessages;

        private Int64 currentUserId = 1;

        private void Init()
        {

            // A --> B without message
            JoinUserChatMessage c1 = new JoinUserChatMessage();
            c1.ChatId = 1;
            c1.Created = DateTime.Now;
            c1.AuthorId = currentUserId;
            c1.ReceiverId = 2;
            joinedUserChatMessages.Add(c1);
            // B --> A with Message 
            JoinUserChatMessage c2 = new JoinUserChatMessage();
            c2.ChatId = 2;
            c2.Created = DateTime.Now;
            c2.AuthorId = 2;
            c2.ReceiverId = currentUserId;
            c2.MessageId = 101;
            c2.Message = "Hello A greetings B (B-->A)";
            joinedUserChatMessages.Add(c2);
            // C --> A with Message 
            JoinUserChatMessage c3 = new JoinUserChatMessage();
            c3.ChatId = 2;
            c3.Created = DateTime.Now;
            c3.AuthorId = 3;
            c3.ReceiverId = currentUserId;
            c3.MessageId = 101;
            c3.Message = "Hello A greetings C (C-->A)";
            joinedUserChatMessages.Add(c3);
            // C --> A without AuthorId (filtered)
            JoinUserChatMessage c4 = new JoinUserChatMessage();
            c4.ChatId = 2;
            c4.Created = DateTime.Now;
            c4.ReceiverId = currentUserId;
            c4.MessageId = 101;
            c4.Message = "Hello A greetings C (C-->A)";
            joinedUserChatMessages.Add(c4);

        }

        public TestChatUserMessage()
        {
            joinedUserChatMessages = new List<JoinUserChatMessage>();
            Init();
        }


        [Fact]
        public void FromJoinUserChatMessagesList()
        {

            ICollection<ChatUserMessage> chatUsers = ChatUserMessage.FromJoinUserChatMessagesList(joinedUserChatMessages);

            foreach (ChatUserMessage chatUser in chatUsers)
            {
                if (chatUser?.userId == null)
                {
                    Assert.Fail("FromJoinUserChatMessagesList returns null authorId!");
                }
            }
            foreach (ChatUserMessage chatUser in chatUsers)
            {
                if (chatUser?.message == null)
                {
                    Assert.Fail("FromJoinUserChatMessagesList returns null message!");
                }
            }

        }

    }
}
