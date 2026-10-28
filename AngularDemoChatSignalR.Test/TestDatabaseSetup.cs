using AngularDemoChatSignalR.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AngularDemoChatSignalR.Test
{
    public static class TestDatabaseSetup
    {

        public static String connectionString;
        public static DatabaseContext? dbContext;
        private static bool wasSetUp;


        private static string findRootDirectory(string rootName, string current)
        {
            if (Path.GetFileName(current).Equals(rootName))
            {
                return current;
            }
            else
            {
                return findRootDirectory(rootName, Directory.GetParent(current).FullName);
            }
        }

        public static void setupTestDatabase(bool releaseDbCOntext = false)
        {
            /*if(wasSetUp)
            {
                return;
            }
            wasSetUp = true;*/
            string serverSettings = Path.Combine(findRootDirectory("AngularDemoChatSignalR", Directory.GetCurrentDirectory()), "AngularDemoChatSignalR.Server");

            Assert.True(Path.Exists(serverSettings), $"Path doesn't exist: {serverSettings.ToString()}");

            // Loading Server Projects appsettings
            var configuration = new ConfigurationBuilder()
            .SetBasePath(serverSettings)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            // User Server Projects Conenction integrationtest
            connectionString =
                configuration.GetConnectionString("integrationtest")
                ?? throw new InvalidOperationException(
                    "ConnectionString 'integrationtest' wurde nicht gefunden.");

            var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlServer(connectionString)
            .Options;


            dbContext = new DatabaseContext(options);

            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();
            if (releaseDbCOntext)
            {
                dbContext.Database.CloseConnection();
                dbContext.Dispose();
                dbContext = null;
            }
        }

    }
}
