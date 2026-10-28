using AngularDemoChatSignalR.Server.Database;
using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Server.Entities;
using AngularDemoChatSignalR.Server.RESTEntities;
using AngularDemoChatSignalR.Server.Tests.UnitTests.Database.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using User = AngularDemoChatSignalR.Server.Database.Entities.User;

namespace AngularDemoChatSignalR.Test.API
{
    [Collection("TestRepositoryBase")] //Sequencial same db-tables
    [TestCaseOrderer(typeof(TestMethodOrderAlphabet))]
    public class TestChatControllerBase : TestAPIBase, IClassFixture<WebApplicationFactory<Program>>, IDisposable
    {
        //BASE WebApplicationFactory<Program> _factory
        //BASE DatabaseContext? dbContext;

        protected  HttpClient? client = null;
        private  UserRepository? repository = null;

        private string state = "beforeInit";

        public static List<User> testUsers = new List<User>();

        public TestChatControllerBase(TestDatabaseWebApplicationFactory<Program> factory) : base(factory)
        {
            repository = new UserRepository(dbContext);
            Before(); // Runs a single time
        }

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

        private void cleanDbTmpEntities()
        {
            try
            {
                dbContext.UserChatMessages.ExecuteDelete();
                System.Console.WriteLine("Cleaned UserChatMessages...");
            }
            catch (Exception ex)
            {
            }
            try
            {
                dbContext.UserChats.ExecuteDelete();
                System.Console.WriteLine("Cleaned UserChatMessages...");
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
            try
            {
                dbContext.Users.ExecuteDelete();
                System.Console.WriteLine("Cleaned Users...");
            }
            catch (Exception ex)
            {
            }

        }
        private void Before()
        {
            try
            {
                setupDbUser();
                state = "afterSetup - Before()";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task After()
        {
            System.Console.WriteLine("Cleaning testdatabase...");
            cleanDbTmpEntities();
            System.Console.WriteLine("Cleaned testdatabase...");
            
        }

        public void Dispose()
        {
            // Cleanup after the test
            After(); // Runs when all facts have finished
            state = "after - Dispose()";
        }

        protected void CreateClient()
        {
            if (client == null)
            {
                client = _factory.CreateClient();
            }
        }


        protected async Task CheckPreTestRequirements(bool checkCurrentUser = true)
        {
            string database = dbContext.Database.GetDbConnection().Database;
            Assert.True("demoNgSigRtest".Equals(database), "Test doesn't use test database!");

            Assert.True(testUsers.Count == 3, "testUsers not loaded into db! Before()");
            Assert.True(repository.CountAll() == 3, "testUsers not loaded into db! Before()");
            Assert.True(repository.GetUserWithSmallestId() > -1, "GetUserWithSmallestId() Fails");
            Assert.True(dbContext.Users.Any(u => u.UserId == testUsers[0].UserId), $"User {testUsers[0].UserId} is missing before HTTP request");

            

            if (checkCurrentUser)
            {
                //Current User needs to be testUsers[0] for all tests.
                const string path = "/api/Chat/currentuser";
                CreateClient();
                var response = await client.GetAsync(path);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(
                        $"GET {path} failed with {(int)response.StatusCode} " +
                        $"{response.StatusCode}\nResponse body: {body}");
                }
                response.EnsureSuccessStatusCode();
                ChatUser? user = await response.Content.ReadFromJsonAsync<ChatUser?>();
                Assert.True(user != null, $"REST Endpoint GetCurrentUser() ==> '{path}' returned null");
                Assert.True(user.userId == testUsers[0].UserId, $"REST Endpoint GetCurrentUser() ==> '{path}' is not {testUsers[0].UserName}");
            }
        }

    }
}
