using AngularDemoChatSignalR.Server.Database;
using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Xunit.v3;
namespace AngularDemoChatSignalR.Server.Tests.UnitTests.Database.Repositories
{
 
    public class TestRepositoryBase
    {

        public DatabaseContext dbContext;


        private string findRootDirectory(string rootName, string current)
        {
            if(Path.GetFileName(current).Equals(rootName))
            {
                return current;
            }
            else
            {
                return findRootDirectory(rootName, Directory.GetParent(current).FullName);
            }
        }

        public TestRepositoryBase() {

            string serverSettings = Path.Combine(findRootDirectory("AngularDemoChatSignalR", Directory.GetCurrentDirectory()), "AngularDemoChatSignalR.Server");

            Assert.True(Path.Exists(serverSettings), $"Path doesn't exist: {serverSettings.ToString()}");

            // Loading Server Projects appsettings
            var configuration = new ConfigurationBuilder()
            .SetBasePath(serverSettings)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            // User Server Projects Conenction integrationtest
            string connectionString =
                configuration.GetConnectionString("integrationtest")
                ?? throw new InvalidOperationException(
                    "ConnectionString 'integrationtest' wurde nicht gefunden.");

            var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connectionString)
            .Options;


            this.dbContext = new DatabaseContext(options);

            this.dbContext.Database.EnsureDeleted();
            this.dbContext.Database.Migrate();
        }

    }
}
