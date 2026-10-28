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
    public class TestChatControllerOpen : TestChatControllerBase
    {
        public TestChatControllerOpen(TestDatabaseWebApplicationFactory<Program> factory) : base(factory)
        {

        }

        [Fact]
        public async Task GetOpenTest()
        {
            await CheckPreTestRequirements();
            // Open chat with testUserB (A --> B)
            long partnerId = testUsers[1].UserId;
            string path = "/api/Chat/open?partnerId=" + partnerId;
            WebApplicationFactoryClientOptions clientOptions = new WebApplicationFactoryClientOptions();

            CreateClient();

            var response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();

            ChatUserState? openedResponse = await response.Content.ReadFromJsonAsync<ChatUserState?>();

            Assert.True(openedResponse != null, $"REST Endpoint GetOpen() ==> '{path}' returned null");
            Assert.True(openedResponse.id == partnerId, $"REST Endpoint GetOpen() ==> '{path}' not responed correct partnerId");
            Assert.True(openedResponse.status == ChatUserState.STATE_SUCCEEDED, $"REST Endpoint GetOpen() ==> '{path}' not responed correct result state");
        }
    }
}
