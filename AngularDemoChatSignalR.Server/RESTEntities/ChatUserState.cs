namespace AngularDemoChatSignalR.Server.RESTEntities
{
    /// <summary>
    /// Same structur in Frontend: angulardemochatsignalr.client\src\app\chat\ChatUserState.ts
    /// </summary>
    public class ChatUserState
    {
        public static string STATE_SUCCEEDED = "OK";
        public static string STATE_FAIL = "FAIL";
        public DateTime dateTime {  get; set; }

        public string status { get; set; }
        public string? name { get; set; }
        public Int64? id { get; set; }
        
        public string? reason { get; set; }


        public ChatUserState() {
            dateTime = DateTime.Now;
            status = STATE_FAIL;
        }

        /// <summary>
        /// Fail with reason constructor
        /// </summary>
        /// <param name="reason"></param>
        public ChatUserState(string reason)
        {
            dateTime = DateTime.Now;
            status = STATE_FAIL;
            this.reason = reason;
        }

        public ChatUserState(DateTime dateTime, string name, Int64 id)
        {
            this.dateTime = dateTime;
            this.name = name;
            this.id = id;
            this.status = STATE_SUCCEEDED;
        }

    }
}
