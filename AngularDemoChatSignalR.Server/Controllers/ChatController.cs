using AngularDemoChatSignalR.Server.Database;
using AngularDemoChatSignalR.Server.Database.Entities;
using AngularDemoChatSignalR.Server.Database.Repositories;
using AngularDemoChatSignalR.Server.Entities;
using AngularDemoChatSignalR.Server.HubsWebsocket;
using AngularDemoChatSignalR.Server.RESTEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;

namespace AngularDemoChatSignalR.Server.Controllers
{
    /// <summary>
    /// Contains all the business logic for http REST requests from angulardemochatsignalr.client\src\app\chat\chat.component.ts
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private UserRepository userRepository;
        private UserChatRepository userChatRepository;
        private UserChatMessageRepository userChatMessageRepository;

        private RealtimeNotificationHubService realtimeNotificationHubService;


        /*private static readonly ChatUser[] DemoUsers = new[]
        {
            new ChatUser(1, "Tom"), new ChatUser(2, "Max", "How are you?"),
            new ChatUser(3,"John"), new ChatUser(4,"John"),
            new ChatUser(5,"Anna"), new ChatUser(6,"Julia"),
            new ChatUser(7,"Pedro"), new ChatUser(8,"David"),
            new ChatUser(9,"Martin"), new ChatUser(10,"Fabian")
        };*/

        private readonly ILogger<ChatController> _logger;

        

        private static Int64 MY_USER_ID;
        private static bool USER_FIRST_INIT = true;

        private Int64 GetCurrentUserId()
        {
            //todo demo-code
            return MY_USER_ID;
        }

        
        [MethodImpl(MethodImplOptions.Synchronized)] // like Java public 'syncronized'
        private void SetChangeUserId(Int64 newUserId)
        {
            _logger.LogInformation($"Changed user {MY_USER_ID} to {newUserId}");
            //todo demo-code
            MY_USER_ID = newUserId;
        }
        

        public ChatController(ILogger<ChatController> logger, DatabaseContext context, RealtimeNotificationHubService realtimeNotificationHubService)
        {
            _logger = logger;
            this.realtimeNotificationHubService = realtimeNotificationHubService;
            userChatRepository = new UserChatRepository(context);
            userRepository = new UserRepository(context);
            userChatMessageRepository = new UserChatMessageRepository(context);

            //todo demo-code init-db
            if (USER_FIRST_INIT)
            {
                MY_USER_ID = userRepository.GetUserWithSmallestId();
                USER_FIRST_INIT = false;
                Console.WriteLine("USER_ID: " + MY_USER_ID);
            }

        }


        /// <summary>
        /// Start a new chat (create the relation author receiver in UserChats)
        /// </summary>
        /// <param name="partnerId">UserId of chat partner</param>
        /// <returns>ChatUserState {status, reason}</returns>
        [HttpGet("open")]
        public ChatUserState GetOpen(Int64 partnerId)
        {
            ChatUserState result = new ChatUserState();
            _logger.LogInformation($"Open chat with {partnerId}");
            if (partnerId > 0 && partnerId != GetCurrentUserId())
            {
                try
                {
                    Int64 curUId = GetCurrentUserId();
                    UserChat user = new(curUId, partnerId);
                    Task task = userChatRepository.AddUserChat(user);
                    task.Wait();
                    result.id = partnerId;
                    result.status = ChatUserState.STATE_SUCCEEDED;
                    // ############## REALTIME SIGNAL-R ##########################
                    // Notify the chatpartner, that there is new data available under /api/Chat/usermessages
                    _ = this.realtimeNotificationHubService.
                        SendRestUpdateNotification(partnerId, "api/chat/usermessages"  /* API_CHAT_USERMESSAGES in chat.component.ts */);

                    // Notify other open instances of this user for refetching
                    _ = this.realtimeNotificationHubService.
                        SendRestUpdateNotification(curUId, "api/chat/usermessages"  /* API_CHAT_USERMESSAGES in chat.component.ts */);

                    // ############## END REALTIME SIGNAL-R ######################
                }
                catch (Exception ex)
                {
                    result.reason = ex.Message;
                    result.status = ChatUserState.STATE_FAIL;
                    //UNIQUE INSERT Exception... chat is already open
                    _logger.LogInformation($"Prevented opening chat {GetCurrentUserId()} to {partnerId} again.");
                }
            }
            return result;
        }

        /// <summary>
        /// Get all messages for the current chat (with author=userId receiver=partnerId or in reversed combination)
        /// </summary>
        /// <param name="userId">Id of the current user</param>
        /// <param name="partnerId">Id of the users chat-partner</param>
        /// <returns>List of ChatUserMessage ordered ASC</returns>
        [HttpGet("usermessages")]
        public IEnumerable<ChatUserMessage> GetUserMessages(Int64 userId, Int64 partnerId)
        {
            if (userId > 0)
            {
                SetChangeUserId(userId);
            }
            Int64 uid = GetCurrentUserId();
            _logger.LogInformation($"Request chats of user {userId} --> {uid}");
            return ChatUserMessage.FromJoinUserChatMessagesList(userChatMessageRepository.GetAllUserChatMessagesWithPartnerById(uid, partnerId));
        }

        private string trimLength(string str, int len)
        {
            if (str.Length > len)
            {
                return str.Substring(0, len)+"...";
            }
            return str;
        }
        /// <summary>
        /// Send a message to the chatPartner. chatUserMessage contains partnerId.
        /// </summary>
        /// <param name="chatUserMessage">Contains partnerId and message</param>
        /// <returns>ChatUserState {status,[id], [reason]} </returns>
        [HttpPost("sendmessage")]
        public ChatUserState PostUserChatMessage([FromBody] ChatUserMessage chatUserMessage)
        {
            Int64 partnerId = chatUserMessage.userId;
            string message = chatUserMessage.message;
            try
            {
                Int64 uid = GetCurrentUserId();
                _logger.LogDebug($"Send msg {uid} --> {partnerId} : '{trimLength(message, 30)}'");
                Task<UserChatMessage> t = this.userChatMessageRepository.AddUserChatMessage(
                    userChatRepository.GetOrCreateSenderChatId(uid, partnerId),
                    message);
                t.Wait();
                UserChatMessage ucmsg = t.Result;

                // ############## REALTIME SIGNAL-R ##########################
                // Notify the chatpartner, that there is new data available under /api/Chat/usermessages
                _ = this.realtimeNotificationHubService.
                    SendRestUpdateNotification(partnerId, "api/chat/usermessages"  /* API_CHAT_USERMESSAGES in chat.component.ts */);
                // ############## END REALTIME SIGNAL-R ######################

                return new ChatUserState(ucmsg.Created, ucmsg.GetType().Name, ucmsg.Id);
            }
            catch (Exception ex)
            {
                return new ChatUserState(ex.Message);
            }
        }
        /// <summary>
        /// Get a list of all open user chats. DEMOCODE setting userId will cange the user for demonstatration.
        /// </summary>
        /// <param name="userId">Id of the current user or -1 so the server does not change the current user.</param>
        /// <returns></returns>
        [HttpGet("userchats")]
        public IEnumerable<ChatUser> GetUserChats(Int64 userId) //todo domo-code no real app would enable a query to user chats by http-params for security reasons.
        {
            
            if (userId > 0)
            {
                Int64 prevuid = GetCurrentUserId();
                SetChangeUserId(userId); //todo demo-code

                // ############## REALTIME SIGNAL-R ##########################
                // Notify that there is new data available under /api/Chat/currentuser --> All clients of previous user change to new id.
                _ = this.realtimeNotificationHubService.
                    SendRestUpdateNotification(prevuid, "api/Chat/currentuser"  /* API_CHAT_CURRENTUSER in chat.component.ts */);
                // ############## END REALTIME SIGNAL-R ######################

            }
            Int64 uid = GetCurrentUserId();
            _logger.LogInformation($"Request chats of user {userId} --> {uid}");
            return ChatUser.FromJoinUserChatList(userChatRepository.GetAllChatsByUserId(uid), uid);
        }
        /// <summary>
        /// Get a list of all users.
        /// </summary>
        /// <returns>List of ChatUser</returns>
        [HttpGet("users")]
        public IEnumerable<ChatUser> GetUsers()
        {
            Task<List<User>> task = userRepository.GetAllUsersExceptCurrent(GetCurrentUserId());
            task.Wait();
            return ChatUser.FromJoinUserList(task.Result);
        }
        /// <summary>
        /// Get the current User.
        /// </summary>
        /// <returns>ChatUser</returns>
        [HttpGet("currentuser")]
        public async Task<ChatUser> GetCurrentUser()
        {
            User usr = await userRepository.GetUserById(GetCurrentUserId());
            return new ChatUser(usr.UserId, usr?.UserName ?? "");
        }
    }
}
