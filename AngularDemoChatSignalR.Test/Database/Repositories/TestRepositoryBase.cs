using AngularDemoChatSignalR.Server.Database;
using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Test;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Xunit.v3;
namespace AngularDemoChatSignalR.Server.Tests.UnitTests.Database.Repositories
{

    [CollectionDefinition("TestRepositoryBase", DisableParallelization = true)]
    public class TestRepositoryBase
    {
        internal DatabaseContext dbContext;

        public TestRepositoryBase() {
            TestDatabaseSetup.setupTestDatabase();
            this.dbContext = TestDatabaseSetup.dbContext;
        }

    }
}
