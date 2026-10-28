using AngularDemoChatSignalR.Server.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;


namespace AngularDemoChatSignalR.Test.API
{
    public class TestDatabaseWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly string connectionString;
        //internal DatabaseContext dbContext;
        public TestDatabaseWebApplicationFactory(): base()
        {
            TestDatabaseSetup.setupTestDatabase(true); //true -> releases the dbContext so the app can connect;
            connectionString = TestDatabaseSetup.connectionString;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services => {
                // Remove ApplicationDbContext registration.
                List<ServiceDescriptor> descriptors = services.Where(
                    d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>)).ToList<ServiceDescriptor>();

                if (!descriptors.IsNullOrEmpty())
                {
                    foreach (var item in descriptors)
                    {
                        services.Remove(item);
                    }
                }

                // Add ApplicationDbContext using an in-memory database for testing.
                services.AddDbContext<DatabaseContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                });
            });
        }

        
    }
}
