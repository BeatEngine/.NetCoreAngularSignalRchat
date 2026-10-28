using AngularDemoChatSignalR.Server.Entities;
using AngularDemoChatSignalR.Server.RESTEntities;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AngularDemoChatSignalR.Test.API
{
    [Collection("TestRepositoryBase")] //Sequencial same db-tables
    public class TestChatControllerPostUserChatMessage : TestChatControllerBase
    {
        public TestChatControllerPostUserChatMessage(TestDatabaseWebApplicationFactory<Program> factory) : base(factory)
        {

        }

        private readonly string testMessage = "Test send msg A --> B!";

        private async Task checkUserMessages(long partnerId)
        {
            Assert.True(partnerId == testUsers[1].UserId, $"Setup of {this.GetType().Name} has changed!");
            string path = "/api/Chat/usermessages?userId=-1&partnerId=" + partnerId; //todo democode userId=-1 (not necessery)
            WebApplicationFactoryClientOptions clientOptions = new WebApplicationFactoryClientOptions();

            CreateClient();

            var response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();

            List<ChatUserMessage?>? userChatsMessages = await response.Content.ReadFromJsonAsync<List<ChatUserMessage?>?>();
            
            Assert.True(userChatsMessages != null, $"REST Endpoint GetUserMessages() ==> '{path}' returned null");

            Assert.Equal(partnerId, testUsers[1].UserId);
            bool equals = false;
            string unequal = "";
            ChatUserMessage filtered = new ChatUserMessage();
            foreach (ChatUserMessage message in userChatsMessages)
            {
                if (message.userId /*authorId*/ != testUsers[0].UserId)
                {
                    continue;
                }
                if (message.message != testMessage)
                {
                    unequal = $"{testMessage} != {message.message}";
                    continue;
                }
                equals = true;
                filtered = message;
                break;
            }
            Assert.True(equals, $"REST Endpoint GetUserMessages() ==> Message author {filtered.userId} {unequal} in result of '{path}'");
        }

        private async Task checkUserChats(long partnerId)
        {
            Assert.True(partnerId == testUsers[1].UserId, $"Setup of {this.GetType().Name} has changed!");
            string path = "/api/Chat/userchats?userId=-1"; //todo democode Server knows our Id --> -1
            WebApplicationFactoryClientOptions clientOptions = new WebApplicationFactoryClientOptions();

            CreateClient();

            var response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();

            List<ChatUser?>? userChats = await response.Content.ReadFromJsonAsync<List<ChatUser?>?>();

            Assert.True(userChats != null, $"REST Endpoint GetUserChats() ==> '{path}' returned null");

            bool equals = false;
            string unequal = "";
            ChatUser filtered = new ChatUser();
            foreach (ChatUser user in userChats)
            {
                if (user.userId != testUsers[1].UserId)
                {
                    continue;
                }
                if (user.userName != testUsers[1].UserName)
                {
                    unequal = $"{testUsers[1].UserName} != {user.userName}";
                    continue;
                }
                equals = true;
                filtered = user;
                break;
            }
            Assert.True(equals, $"REST Endpoint GetUserChats() ==> TestUser {testUsers[1].UserName} {unequal} in result of '{path}'");
            Assert.True(testMessage.Equals(filtered.previewMessage), $"REST Endpoint GetUserChats() ==> TestUser {testMessage} != {filtered.previewMessage} in result of '{path}'");
            

        }

        [Fact]
        public async Task PostUserChatMessageTest()
        {
            await CheckPreTestRequirements();
            // Open chat with testUserB (A --> B)
            long partnerId = testUsers[1].UserId;
            string path = "/api/Chat/sendmessage";
            WebApplicationFactoryClientOptions clientOptions = new WebApplicationFactoryClientOptions();

            CreateClient();
            ChatUserMessage testChatUserMessage = new ChatUserMessage(DateTime.Now, partnerId, testMessage);
            var response = await client.PostAsJsonAsync(path, testChatUserMessage);
            response.EnsureSuccessStatusCode();

            ChatUserState? openedResponse = await response.Content.ReadFromJsonAsync<ChatUserState?>();

            Assert.True(openedResponse != null, $"REST Endpoint PostUserChatMessage() ==> '{path}' returned null");
            Assert.True(openedResponse.id > 0, $"REST Endpoint PostUserChatMessage() ==> '{path}' not responed valid messageId");
            Assert.True(openedResponse.status == ChatUserState.STATE_SUCCEEDED, $"REST Endpoint PostUserChatMessage() ==> '{path}' not responed correct result state");


            await checkUserChats(partnerId);

            await checkUserMessages(partnerId);
        }
    }
}
