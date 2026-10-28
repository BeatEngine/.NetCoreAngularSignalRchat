using AngularDemoChatSignalR.Server.Entities;
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
    public class TestChatControllerCurrentUser : TestChatControllerBase
    {
        public TestChatControllerCurrentUser(TestDatabaseWebApplicationFactory<Program> factory) : base(factory)
        {

        }
        [Fact]
        public async Task GetUsersTest()
        {
            await CheckPreTestRequirements();
            const string path = "/api/Chat/users";
            WebApplicationFactoryClientOptions clientOptions = new WebApplicationFactoryClientOptions();

            CreateClient();

            var response = await client.GetAsync(path);
            response.EnsureSuccessStatusCode();

            List<ChatUser?>? users = await response.Content.ReadFromJsonAsync<List<ChatUser?>?>();

            Assert.True(users != null, $"REST Endpoint GetUsers() ==> '{path}' returned null");

            Assert.True(users.Count == testUsers.Count - 1, $"REST Endpoint GetUsers() ==> '{path}' count wrong expected results excluding own user");

            for (int i = 1; i < testUsers.Count; i++)
            {
                bool equals = false;
                string unequal = "";
                foreach (ChatUser user in users)
                {
                    if (user.userId != testUsers[i].UserId)
                    {
                        continue;
                    }
                    if (user.userName != testUsers[i].UserName)
                    {
                        unequal = $"{testUsers[i].UserName} != {user.userName}";
                        continue;
                    }
                    equals = true;
                    break;
                }
                Assert.True(equals, $"REST Endpoint GetUsers() ==> TestUser {testUsers[i].UserName} {unequal} in result of '{path}'");
            }
        }
    }
}
