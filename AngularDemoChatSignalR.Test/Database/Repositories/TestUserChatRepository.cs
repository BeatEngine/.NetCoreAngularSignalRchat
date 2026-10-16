using AngularDemoChatSignalR.Server.Database;
using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository;
using AngularDemoChatSignalR.Server.Entities;
using AngularDemoChatSignalR.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;
namespace AngularDemoChatSignalR.Server.Tests.UnitTests.Database.Repositories
{
    [Collection("TestRepositoryBase")] //Sequencial same db-tables
    [TestCaseOrderer(typeof(TestMethodOrderAlphabet))]
    public class TestUserChatRepository : TestRepositoryBase, IDisposable 
    {

        private UserChatRepository repository;

        private void Before()
        {
            beforeState = "ran";

            try
            {
                setupDbUser();
                setupDbUserChat();
                setupDbUserChatMessages();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                After();
            }

        }

        private void After()
        {
            System.Console.WriteLine("Cleaning testdatabase...");
            cleanDbTmpEntities();
            System.Console.WriteLine("Cleaned testdatabase...");
        }

        public void Dispose()
        {
            // Cleanup after the test
            After();
        }
        public TestUserChatRepository() : base()
        {

            this.repository = new UserChatRepository(dbContext);

            Before();
        }

        List<User> testUsers = new List<User>();
        List<UserChat> testUsersChats = new List<UserChat>();
        List<UserChatMessage> testUsersChatMessages = new List<UserChatMessage>();

        private string beforeState = "Not ran";

        private void setupDbUser()
        {
            User testUserA = new User();
            testUserA.UserName = "testUserA";
            testUserA.Created = DateTime.Now;
            User testUserB = new User();
            testUserB.UserName = "testUserB";
            testUserB.Created = DateTime.Now;
            User testUserC = new User();
            testUserC.UserName = "testUserC";
            testUserC.Created = DateTime.Now;

            testUsers.Add(dbContext.Add(testUserA).Entity);
            testUsers.Add(dbContext.Add(testUserB).Entity);
            testUsers.Add(dbContext.Add(testUserC).Entity);
            dbContext.SaveChanges();

        }

        private void setupDbUserChat()
        {
            UserChat chatAB = new UserChat();
            chatAB.AuthorId = testUsers[0].UserId;
            chatAB.ReceiverId = testUsers[1].UserId;

            UserChat chatBA = new UserChat();
            chatBA.AuthorId = testUsers[1].UserId;
            chatBA.ReceiverId = testUsers[0].UserId;

            UserChat chatCB = new UserChat();
            chatCB.AuthorId = testUsers[2].UserId;
            chatCB.ReceiverId = testUsers[1].UserId;

            UserChat chatCA = new UserChat();
            chatCA.AuthorId = testUsers[2].UserId;
            chatCA.ReceiverId = testUsers[0].UserId;


            testUsersChats.Add(dbContext.Add(chatAB).Entity);
            testUsersChats.Add(dbContext.Add(chatBA).Entity);
            testUsersChats.Add(dbContext.Add(chatCB).Entity);
            testUsersChats.Add(dbContext.Add(chatCA).Entity);
            dbContext.SaveChanges();
        }

        private void setupDbUserChatMessages()
        {
            UserChatMessage testUserChatMessageAB = new UserChatMessage();
            testUserChatMessageAB.UserChatId = testUsersChats[0].Id;
            testUserChatMessageAB.Message = "Hallo B best regards A  (A-->B)";
            testUserChatMessageAB.Created = DateTime.Now;

            UserChatMessage testUserChatMessageBA = new UserChatMessage();
            testUserChatMessageBA.UserChatId = testUsersChats[1].Id;
            testUserChatMessageBA.Message = "Hallo A best regards B  (B-->A)";
            testUserChatMessageBA.Created = DateTime.Now;

            UserChatMessage testUserChatMessageCB = new UserChatMessage();
            testUserChatMessageCB.UserChatId = testUsersChats[2].Id;
            testUserChatMessageCB.Message = "Hallo B best regards C  (C-->B)";
            testUserChatMessageCB.Created = DateTime.Now;

            UserChatMessage testUserChatMessageCA = new UserChatMessage();
            testUserChatMessageCA.UserChatId = testUsersChats[3].Id;
            testUserChatMessageCA.Message = "Hallo A best regards C  (C-->A)";
            testUserChatMessageCA.Created = DateTime.Now;

            UserChatMessage testUserChatMessageAB2 = new UserChatMessage();
            testUserChatMessageAB2.UserChatId = testUsersChats[0].Id;
            testUserChatMessageAB2.Message = "Message2 (A-->B)";
            testUserChatMessageAB2.Created = DateTime.Now;

            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageAB).Entity);
            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageBA).Entity);
            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageCB).Entity);
            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageCA).Entity);
            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageAB2).Entity);
            dbContext.SaveChanges();

        }

        private void cleanDbTmpEntities()
        {
            try
            {
                dbContext.RemoveRange(testUsersChatMessages);
                dbContext.SaveChanges();
                testUsersChatMessages.Clear();
                System.Console.WriteLine("Cleaned testUsersChatMessages...");
            }
            catch (Exception ex)
            {
            }
            try
            {
                dbContext.RemoveRange(testUsersChats);
                dbContext.SaveChanges();
                testUsersChats.Clear();
                System.Console.WriteLine("Cleaned testUsersChats...");
            }
            catch (Exception ex)
            {
            }
            try
            {
                dbContext.RemoveRange(testUsers);
                dbContext.SaveChanges();
                testUsers.Clear();
                System.Console.WriteLine("Cleaned testUsers...");
            }
            catch (Exception ex)
            {
            }
        }

        private void CheckPreTestRequirements()
        {
            Assert.True(testUsers.Count == 3, "testUsers not loaded into db! Before(): " + beforeState);
            Assert.True(testUsersChats.Count == 4, "testUsersChats not loaded into db! Before(): " + beforeState);
            Assert.True(testUsersChatMessages.Count == 5, "testUsersChatMessages not loaded into db! Before(): " + beforeState);
        }


        [Fact]
        public void Test11GetAllChatsByUserId()
        {
            CheckPreTestRequirements();

            int c = dbContext.UserChatMessages.Count<UserChatMessage>();

            Assert.True(c > 0, "Chats loaded: " + c); 

            // Get all Chats of A
            ICollection<Server.Database.Repositories.JoinUserChatRepository.JoinUserChat> joinUserChats = repository.GetAllChatsByUserId(testUsers[0].UserId);

            // Does NOT contain  C --> B
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChat row in joinUserChats)
            {
                if (row.AuthorId != testUsers[0].UserId && row.ReceiverId != testUsers[0].UserId)
                {
                    Assert.Fail($"GetAllChatsByUserId({testUsers[0].UserId}) returned AuthorId={row.AuthorId} ReceiverId={row.ReceiverId} --> unrelated chat!");
                }
            }

            // A --> B, B --> A, C --> A NOT second A --> B
            Assert.True(joinUserChats.Count == 3, "GetAllChatsByUserId() Count: " + joinUserChats.Count + " Expected: 3 (C --> B should not be included)");

            Server.Database.Repositories.JoinUserChatRepository.JoinUserChat? before = null;
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChat row in joinUserChats)
            {
                if(before != null)
                {
                    if(!(before.Created > row.Created)) //NOT DESC
                    {
                        Assert.Fail("Wrong order. Not updated DESC (before to this: {last.Created} to {row.Created} )");
                    }
                }
                before = row;
            }

            // Does UserNames not empty
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChat row in joinUserChats)
            {
                if(row.AuthorName == "" || row.ReceiverName == "")
                {
                    Assert.Fail($"GetAllChatsByUserId({testUsers[0].UserId}) returned blank names '{row.AuthorName}' '{row.ReceiverName}' AuthorId={row.AuthorId} ReceiverId={row.ReceiverId} --> unrelated chat!");
                }
            }



        }


        [Fact]
        public void Test22UniqueConstraintAuthorAndReceiver()
        {
            CheckPreTestRequirements();

            UserChat userChat = this.testUsersChats[0];

            bool ThrowsUniqueException = false;
            try
            {
                Task task = this.repository.AddUserChat(new UserChat(userChat.AuthorId, userChat.ReceiverId));
                task.Wait();
            }
            catch (Exception ex)
            {
                ThrowsUniqueException = true;
                Console.WriteLine("Expected unique constrain: " + ex.Message);
            }
            Assert.True(ThrowsUniqueException, "Unique constraint missing on UserChat!");

        }

        
        [Fact]
        public void Test33GetOrCreateSenderChatId()
        {
            CheckPreTestRequirements();

            int cntBefore = dbContext.UserChats.Count<UserChat>();

            //Existing
            long chatId = repository.GetOrCreateSenderChatId(testUsersChats[0].AuthorId, testUsersChats[0].ReceiverId);
            Assert.True(testUsersChats[0].Id == chatId, $"Unequal expected chatId: {testUsersChats[0].Id} but have got: {chatId}");

            int cntMiddle = dbContext.UserChats.Count<UserChat>();
            Assert.Equal(cntBefore, cntMiddle); //Nothing inserted!

            //New 

            User testUserN = new User();
            testUserN.UserName = "GetOrCreateSenderChatId";
            testUserN.Created = DateTime.Now;
            int tuc = testUsers.Count();
            testUsers.Add(dbContext.Add(testUserN).Entity);
            dbContext.SaveChanges();
            int tuc2 = testUsers.Count();
            Assert.True(tuc2 > tuc, "Testuser for GetOrCreateSenderChatId not inserted!");
            // New Chat and return its id
            long chatId2 = repository.GetOrCreateSenderChatId(testUsers[testUsers.Count-1].UserId, testUsers[0].UserId);
            int cntAfter = dbContext.UserChats.Count<UserChat>();
            Assert.True(cntMiddle < cntAfter, "Nothing was inserted!");
            Assert.True(dbContext.UserChats.Where(uc => uc.Id == chatId2).Any(), $"New chat-id: {chatId2} not found!");


        }

        [Fact]
        public void Test44GetAllChatsByUserIdMissingChats()
        {
            CheckPreTestRequirements();

            this.dbContext.RemoveRange(this.testUsersChatMessages);
            this.dbContext.SaveChanges();
            // Get all Chats of A
            ICollection<Server.Database.Repositories.JoinUserChatRepository.JoinUserChat> joinUserChats = repository.GetAllChatsByUserId(testUsers[0].UserId);

            // Messages must be empty in this test
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChat row in joinUserChats)
            {
                if (row.MessageId != null || row.Message != null)
                {
                    Assert.Fail($"Test setup is incorrect: Messages found: [{row.MessageId}]='{row.Message}' ");
                }
            }

            // Does NOT contain  C --> B
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChat row in joinUserChats)
            {
                if (row.AuthorId != testUsers[0].UserId && row.ReceiverId != testUsers[0].UserId)
                {
                    Assert.Fail($"Test33GetAllChatsByUserIdMissingChats({testUsers[0].UserId}) returned AuthorId={row.AuthorId} ReceiverId={row.ReceiverId} --> unrelated chat!");
                }
            }

            // A --> B, B --> A, C --> A NOT second A --> B
            Assert.True(joinUserChats.Count == 3, "Test33GetAllChatsByUserIdMissingChats() Count: " + joinUserChats.Count + " Expected: 3 (C --> B should not be included)");

            // UserNames are not empty
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChat row in joinUserChats)
            {
                if (row.AuthorName == "" || row.ReceiverName == "")
                {
                    Assert.Fail($"Test33GetAllChatsByUserIdMissingChats({testUsers[0].UserId}) returned blank names '{row.AuthorName}' '{row.ReceiverName}' AuthorId={row.AuthorId} ReceiverId={row.ReceiverId} --> unrelated chat!");
                }
            }

            ICollection<ChatUser> chatUsers = ChatUser.FromJoinUserChatList(joinUserChats, testUsers[0].UserId);
            foreach (ChatUser chatUser in chatUsers)
            {
                if (chatUser.userName == "")
                {
                    Assert.Fail($"FromJoinUserChatList returns empty UserNames: chatUser.userName = '{chatUser.userName}'");
                }
            }

        }


    }
}
