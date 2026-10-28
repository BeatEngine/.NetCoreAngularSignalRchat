using AngularDemoChatSignalR.Server.Database;
using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Test;
using AngularDemoChatSignalR.Test.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Reflection;
using Xunit.v3;
namespace AngularDemoChatSignalR.Server.Tests.UnitTests.Database.Repositories
{
    [CollectionDefinition("TestRepositoryBase", DisableParallelization = true)]
    public class TestAPIBase : IClassFixture<TestDatabaseWebApplicationFactory<Program>>
    {
        protected readonly TestDatabaseWebApplicationFactory<Program> _factory;
        protected readonly DatabaseContext dbContext;
        protected readonly IServiceScope _scope;

        public TestAPIBase(TestDatabaseWebApplicationFactory<Program> factory) :base()
        {
            _factory = factory;
            _scope = _factory.Services.CreateScope();
            dbContext = _scope.ServiceProvider.GetServices<DatabaseContext>().Single();
        }

    }
}
