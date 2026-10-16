using AngularDemoChatSignalR.Server.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace AngularDemoChatSignalR.Server.Database.Repositories
{
    public class UserRepository
    {

        private DatabaseContext dbContext;

        public UserRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<User>> AddUser(User user)
        {
            // SQL   INSERT ON CONFLICT UPDATE
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<User> entityEntry = await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();
            return entityEntry;
        }

        public long CountAll()
        {
            return dbContext.Users.LongCount();

        }

        public async Task<List<User>> GetAllUsersExceptCurrent(Int64 currentUserId)
        {
            return await dbContext.Users.Where(u => u.UserId != currentUserId).ToListAsync<User>();
        }

        /// <summary>
        /// <c>Attention</c> you need to remove UserChats manually before!
        /// </summary>
        public async Task RemoveUser(User user)
        {
            // SQL   Remove User
            dbContext.Remove(user);
            await dbContext.SaveChangesAsync();
        }

        public long GetUserWithSmallestId()
        {
            try
            {
                return dbContext.Users.OrderBy(x => x.UserId).Select(x => x.UserId).Take(1).First();
            }
            catch(Exception e)
            {
                return -1;
            }
        }

        public async Task<User> GetUserById(Int64 userId)
        {
            return await dbContext.Users.Where(u => u.UserId == userId).SingleAsync<User>();
        }
    }
}
