using AngularDemoChatSignalR.Server.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace AngularDemoChatSignalR.Server.Database
{
    public class DatabaseContext : DbContext
    {
        
        /* SQL SETUP Model
         
            USER: PK=Id

            USERCHAT: PK=Id UNIQUE_KEY=USER_PK(author)+USER_PK(receiver)  

                       get all messages for chat with currPartnerId --> SELECT ...   (author = partnerId and receiver = userId)
                                                                                  or (author = userId and receiver = partnerId) join ...

                       get all my open chats with last message --> SELECT MAX(USERCHATMESSAGE.created) author,  
              SELECT DISTINCT chatPartnerId, author 
                ( ( SELECT receiverId AS chatPartnerId FROM USERCHAT (author is userId) UNION SELECT ... FROM USERCHAT author AS chatPartnerId (receiver is userId) )
                                                     ) left join USERCHATMESSAGE on id... GROUP BY author

            USERCHATMESSAGE: PK=Id, FK=USERCHAT_PK, Message, created, updated
         
         
         */

        public DbSet<User> Users { get; set; }

        public DbSet<UserChat> UserChats { get; set; }

        public DbSet<UserChatMessage> UserChatMessages { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users").
                HasMany<UserChat>(a => a.AuthorUserChats).
                WithOne(a => a.Author).HasForeignKey(c => c.AuthorId).IsRequired(true).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<User>().
                HasMany<UserChat>(r => r.ReceiverUserChats).
                WithOne(r => r.Receiver).HasForeignKey(c => c.ReceiverId).IsRequired(true).OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<UserChat>().ToTable("UserChat").
                HasMany<UserChatMessage>(m => m.UserChatMessages).
                WithOne(c => c.UserChat).HasForeignKey(m => m.UserChatId).IsRequired(true); 

            modelBuilder.Entity<UserChat>().HasIndex(x => new { x.AuthorId, x.ReceiverId }).IsUnique(true);//No duplicated combis author and receiver constranint

            modelBuilder.Entity<UserChatMessage>().ToTable("UserChatMessage");
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
