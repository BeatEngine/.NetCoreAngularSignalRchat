using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;

namespace AngularDemoChatSignalR.Server.Database.Repositories
{

    public class UserChatRepository
    {

        private DatabaseContext dbContext;

        public UserChatRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddUserChat(UserChat user)
        {
            // SQL   INSERT ON CONFLICT UPDATE
            await dbContext.AddAsync(user);
            await dbContext.SaveChangesAsync();
        }


        public ICollection<JoinUserChat> GetAllChatsByUserId(Int64 userId)
        {
            //return dbContext.UserChats.Where(c => c.ReceiverId == userId || c.AuthorId == userId).Distinct( )

            var query =
                from chat in dbContext.UserChats.Where(c => c.AuthorId == userId || c.ReceiverId == userId)
                join message in dbContext.UserChatMessages
                    on chat.Id equals message.UserChatId into messages
                join author in dbContext.Users on chat.AuthorId equals author.UserId
                join receiver in dbContext.Users on chat.ReceiverId equals receiver.UserId
                from message in messages
                    .OrderByDescending(m => m.Created)
                    .Take(1)
                    .DefaultIfEmpty()
                select new JoinUserChat
                {
                    ChatId = chat.Id,
                    MessageId = message.Id,
                    AuthorId = chat.AuthorId,
                    ReceiverId = chat.ReceiverId,
                    AuthorName = author.UserName,
                    ReceiverName = receiver.UserName,
                    Created = message.Created,
                    Message = message.Message,
                };

            return query.OrderByDescending(x => x.Created).ToList();
            
        }

        public Int64 GetOrCreateSenderChatId(Int64 userId, Int64 receiverId)
        {
            List<UserChat> userChats = dbContext.UserChats.Where(c => c.AuthorId == userId && c.ReceiverId == receiverId).ToList();
            if(userChats.Count > 1)
            {
                throw new Exception("Database integrity violation !!! Table 'UserChat' unique constraint (authorId and receiverId) missing! This should never been thrown.");
            }
            Int64 id;
            if(userChats.Count == 0)
            {
                Microsoft.EntityFrameworkCore.ChangeTracking.
                    EntityEntry<UserChat> entityEntry = dbContext.Add(new UserChat(userId, receiverId));
                dbContext.SaveChanges();
                id = entityEntry.Entity.Id;
            }
            else
            {
                id = userChats[0].Id;
            }
            return id;
        }



        /// <summary>
        /// <c>Attention</c> one UI-Chat can consist of two UserChat.
        /// This removes both relations
        /// <c>{author: userIdA, receiver: userIdB} and {author: userIdB, receiver: userIdA}</c>
        /// </summary>
        public async Task RemoveUserChat(BigInteger userIdA, BigInteger userIdB)
        {
            await dbContext.UserChats.Where(c => (c.AuthorId == userIdA && c.ReceiverId == userIdB)
            || (c.AuthorId == userIdB && c.ReceiverId == userIdA)
            ).ExecuteDeleteAsync();
        }

        /// <summary>
        /// <c>Attention</c> one UI-Chat can consist of two UserChat 
        /// <c>{author: 1, receiver: 2} and {author: 2, receiver: 1}</c>
        /// </summary>
        public async Task RemoveUserChat(UserChat user)
        {
            // SQL   Remove User
            dbContext.Remove(user);
            await dbContext.SaveChangesAsync();
        }



        

    }
}
