using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Server.Tests.UnitTests.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularDemoChatSignalR.Test.Database.Repositories
{
    [Collection("TestRepositoryBase")] //Sequencial same db-tables
    public class TestUserRepository : TestRepositoryBase, IDisposable
    {

        private UserRepository repository;

        private void Before()
        {
            beforeState = "ran";

            try
            {
                setupDbUser();
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
        public TestUserRepository() : base()
        {

            this.repository = new UserRepository(dbContext);

            Before();
        }

        List<User> testUsers = new List<User>();

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

        private void cleanDbTmpEntities()
        {
            try
            {
                dbContext.RemoveRange(testUsers);
                dbContext.SaveChanges();
                testUsers.Clear();
                System.Console.WriteLine("Cleaned testUsersChatMessages...");
            }
            catch (Exception ex)
            {
            }
        }

        [Fact]
        public void TestCounts()
        {
            Assert.True(testUsers.Count == 3, "testUsers not loaded into db! Before(): " + beforeState);

            int c = dbContext.Users.Count<User>();

            Assert.True(c > 0, "No users loaded: " + c);
            Assert.True(c == 3, "Expected 3 users loaded: " + c);

            Assert.Equal(c, repository.CountAll());

        }
        [Fact]
        public void TestGetUserWithSmallestId()
        {
            Assert.Equal(testUsers[0].UserId, repository.GetUserWithSmallestId());
        }


    }
}
