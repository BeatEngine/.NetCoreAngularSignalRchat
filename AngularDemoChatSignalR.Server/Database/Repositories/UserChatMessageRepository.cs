using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories.JoinUserChatRepository;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace AngularDemoChatSignalR.Server.Database.Repositories
{
    public class UserChatMessageRepository
    {

        private DatabaseContext dbContext;

        public UserChatMessageRepository(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddUserChatMessage(UserChatMessage message)
        {
            // SQL   INSERT ON CONFLICT UPDATE
            await dbContext.AddAsync(message);
            await dbContext.SaveChangesAsync();
        }

        public async Task<UserChatMessage> AddUserChatMessage(Int64 chatId, String text)
        {
            UserChatMessage userChatMessage = new UserChatMessage(chatId, text);
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserChatMessage> entityEntry = await dbContext.UserChatMessages.AddAsync(userChatMessage);
            await dbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }



        public ICollection<JoinUserChatMessage> GetAllUserChatMessagesWithPartnerById(Int64 userId, Int64 partnerId)
        {

            var query =
                        from chat in dbContext.UserChats.Where(c => c.AuthorId == userId && c.ReceiverId == partnerId || c.AuthorId == partnerId && c.ReceiverId == userId)
                        join message in dbContext.UserChatMessages
                            on chat.Id equals message.UserChatId into messages
                        from message in messages
                            .OrderByDescending(m => m.Created)
                            .DefaultIfEmpty()
                        select new JoinUserChatMessage
                        {
                            ChatId = chat.Id,
                            MessageId = message.Id,
                            AuthorId = chat.AuthorId,
                            ReceiverId = chat.ReceiverId,
                            Created = message.Created,
                            Updated = message.Updated,
                            Message = message.Message,
                        };

            return query.OrderBy(x => x.Created).ToList();
        }


        /// <summary>
        /// <c>Attention</c> one UI-Chat can consist of two UserChat 
        /// <c>{author: 1, receiver: 2} and {author: 2, receiver: 1}</c>
        /// </summary>
        public async Task RemoveUserChatMessageById(BigInteger UserChatMessageId)
        {
            await dbContext.UserChatMessages.Where(m => m.Id == UserChatMessageId).ExecuteDeleteAsync();
        }

    }
}
