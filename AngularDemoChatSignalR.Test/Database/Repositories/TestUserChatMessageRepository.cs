using AngularDemoChatSignalR.Server.Database;
using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository;
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
    public class TestUserChatMessageRepository : TestRepositoryBase, IDisposable 
    {

        private UserChatMessageRepository repository;

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


        public TestUserChatMessageRepository() : base()
        {

            this.repository = new UserChatMessageRepository(dbContext);

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
            testUserB.Created = DateTime.Now.AddMilliseconds(1);
            User testUserC = new User();
            testUserC.UserName = "testUserC";
            testUserC.Created = DateTime.Now.AddMilliseconds(2);

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

            UserChatMessage testUserChatMessageAB2 = new UserChatMessage();
            testUserChatMessageAB2.UserChatId = testUsersChats[0].Id;
            testUserChatMessageAB2.Message = "Message2  (A-->B)";
            testUserChatMessageAB2.Created = DateTime.Now;

            UserChatMessage testUserChatMessageCB = new UserChatMessage();
            testUserChatMessageCB.UserChatId = testUsersChats[2].Id;
            testUserChatMessageCB.Message = "Hallo B best regards C  (C-->B)";
            testUserChatMessageCB.Created = DateTime.Now;

            UserChatMessage testUserChatMessageCA = new UserChatMessage();
            testUserChatMessageCA.UserChatId = testUsersChats[3].Id;
            testUserChatMessageCA.Message = "Hallo A best regards C  (C-->A)";
            testUserChatMessageCA.Created = DateTime.Now;

            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageAB).Entity);
            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageBA).Entity);
            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageAB2).Entity);
            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageCB).Entity);
            testUsersChatMessages.Add(dbContext.Add(testUserChatMessageCA).Entity);
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
        public void Test11GetAllUserChatMessagesWithPartnerById()
        {
            CheckPreTestRequirements();

            int c = dbContext.UserChatMessages.Count<UserChatMessage>();

            Assert.True(c > 0, "Chats loaded: " + c); 

            // Get all Chats of A <--> B
            ICollection<Server.Database.Repositories.JoinUserChatRepository.JoinUserChatMessage> joinUserChatMessages = repository.GetAllUserChatMessagesWithPartnerById(testUsers[0].UserId, testUsers[1].UserId);

            // Does only cointains A to B or B to A Messages!
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChatMessage row in joinUserChatMessages)
            {
                if (!( row.AuthorId == testUsers[0].UserId && row.ReceiverId == testUsers[1].UserId    //NOT (   A --> B
                    || row.AuthorId == testUsers[1].UserId && row.ReceiverId == testUsers[0].UserId )) //     OR B --> A   )
                {
                    Assert.Fail($"GetAllChatsByUserId({testUsers[0].UserId}) returned AuthorId={row.AuthorId} ReceiverId={row.ReceiverId} --> unrelated chat!");
                }
            }

            // SUM OF ALL MSG(A --> B, B --> A)
            Assert.True(joinUserChatMessages.Count == 3, "GetAllUserChatMessagesWithPartnerById() Count: " + joinUserChatMessages.Count + " Expected: 3 (a-->b + b-->a + a-->b)");

            // Check order
            Server.Database.Repositories.JoinUserChatRepository.JoinUserChatMessage? before = null;
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChatMessage row in joinUserChatMessages)
            {
                if (before != null)
                {
                    if (before.Created >= row.Created) //ASCENDING
                    {
                        Assert.Fail($"Wrong order. Not updated ASC (before to this: {before.Created} to {row.Created} )");
                    }
                }
                before = row;
            }


        }

        [Fact]
        public void Test22AddUserChatMessage()
        {
            CheckPreTestRequirements();
            int c = dbContext.UserChatMessages.Count<UserChatMessage>();
            Assert.True(c > 0, "Chats loaded: " + c);
            const string msg = "New inserted!";
            Task<UserChatMessage> task = repository.AddUserChatMessage(testUsersChats[testUsersChats.Count - 1].Id, msg);
            task.Wait();
            testUsersChatMessages.Add(task.Result);
            int c2 = dbContext.UserChatMessages.Count<UserChatMessage>();
            Assert.True(c + 1 == c2, $"AddUserChatMessage count before:{c} count after insert:{c2}");
            Assert.True(dbContext.UserChatMessages.Where(m => msg.Equals(m.Message)).Any(), $"No message with text '{msg}' found!");
            Assert.True(msg.Equals(testUsersChatMessages[testUsersChatMessages.Count - 1].Message), "AddUserChatMessage return is wrong!");
        }

        [Fact]
        public void Test33GetAllUserChatMessagesWithPartnerByIdEmptyMessages()
        {
            CheckPreTestRequirements();
            int c = dbContext.UserChatMessages.Count<UserChatMessage>();
            Assert.True(c > 0, "Chats loaded: " + c);

            this.dbContext.RemoveRange(this.testUsersChatMessages);
            this.dbContext.SaveChanges();

            // Get all Chats of A <--> B
            ICollection<Server.Database.Repositories.JoinUserChatRepository.JoinUserChatMessage> joinUserChatMessages = repository.GetAllUserChatMessagesWithPartnerById(testUsers[0].UserId, testUsers[1].UserId);

            // Does only cointains A to B or B to A Messages!
            foreach (Server.Database.Repositories.JoinUserChatRepository.JoinUserChatMessage row in joinUserChatMessages)
            {
                if (!(row.AuthorId == testUsers[0].UserId && row.ReceiverId == testUsers[1].UserId    //NOT (   A --> B
                    || row.AuthorId == testUsers[1].UserId && row.ReceiverId == testUsers[0].UserId)) //     OR B --> A   )
                {
                    Assert.Fail($"GetAllChatsByUserId({testUsers[0].UserId}) returned AuthorId={row.AuthorId} ReceiverId={row.ReceiverId} --> unrelated chat!");
                }
            }

            // SUM OF MSG(A --> B, B --> A)
            Assert.True(joinUserChatMessages.Count == 2, "GetAllUserChatMessagesWithPartnerById() Count: " + joinUserChatMessages.Count + " Expected: 3 (a-->b + b-->a (not 2. a-->b messages deleted in test!))");

        }

        



    }
}
