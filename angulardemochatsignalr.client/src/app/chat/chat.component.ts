import { HttpClient } from '@angular/common/http';
import * as signalR from "@microsoft/signalr";
import { Component, ElementRef, NgModule, NO_ERRORS_SCHEMA, OnInit, signal, ViewChild, WritableSignal } from '@angular/core';
import { ChatHead } from './chat-head/chat-head.component';
import { ChatItem } from './chat-item/chat-item.component';
import { ChatUserView } from './chat-user-view/chat-user-view.component';
import { ChatUser } from './ChatUserType';
import { ChatUserMessage } from './ChatUserMessageType';
import { ChatUserState } from './ChatUserStateType';

/**
  * Root class of the chat contains a head and opened user-chats.
  * Also includes all the REST endpoint methods fetch... or post...
  * 
  *Chat
  {
  1_____________
   ++ChatHead
   ++ChatItems
  2_____________
   ++ChatUserView
  }
  *
  */
@Component({
  selector: 'chat',
  schemas: [NO_ERRORS_SCHEMA],
  imports: [ChatHead, ChatItem, ChatUserView],
  standalone: true,
  templateUrl: 'chat.component.html',
  styleUrl: 'chat.component.scss'
})
export class Chat {
  /**The users */
  public users: WritableSignal<ChatUser[]>;
  /**The open user chats */
  public userChats: WritableSignal<ChatUser[]>;
  private httpClient: HttpClient;

  /**The current or new id of this user */
  public myChatUserId: WritableSignal<number | null> = signal(null);
  public myChatUser: WritableSignal<ChatUser> = signal({ userId: -1, userName: "", previewMessage: "" });

  /**The User that we are chatting with in ChatUserView  */
  public currentChatPartner: WritableSignal<ChatUser | null> = signal(null);

  public currentChatUserView: WritableSignal<ChatUserView | null> = signal(null);

  //Needed for automatic scrolling to the chat end.
  @ViewChild('userChat')
  private userChatElement!: ElementRef<HTMLDivElement>;

  // --------------------- REST-ENDPOINTS ---------------------------

  // !!!! When a Rest call is made SignalR (websocket) stops for that time SigR then reconnects as implemented automatcally !!!!

  private API_CHAT_OPEN: string = 'api/chat/open';
  private API_CHAT_USERCHATS: string = 'api/chat/userchats';
  private API_CHAT_USERS: string = 'api/chat/users';
  private API_CHAT_CURRENTUSER: string = 'api/chat/currentuser';
  private API_CHAT_USERMESSAGES: string = 'api/chat/usermessages';
  private API_CHAT_SENDMESSAGE: string = 'api/chat/sendmessage';

  /**
   * For the case that the server notifies about updates for API_CHAT_USERMESSAGES
   */
  private triggerRefetchRestMessages() {
    const cuc: ChatUserView | null = this.currentChatUserView();
    // Refetch messages if currentChatUserView is opened (!=null)
    if (cuc != null) {
      cuc.RefetchUserMessages();
    }
    else {
      // The currentChatUserView is closed, but the preview message has changed
      this.fetchUserChats();
    }
  }

  /**
   * For the case that the server notifies about updates for API_CHAT_CURRENTUSER
   */
  private triggerRefetchUserChanged() {
    this.fetchUsers();
    this.fetchUserChats();
    this.fetchCurrentUser();
  }

  /**
   * Triggers to refetch the REST endpoints when the server notifies about new data.
   * @param restEndpointPath like '/api/....'
   */
  private async triggerRefetchRest(restEndpointPath: string) {

    switch (restEndpointPath) {
      case this.API_CHAT_CURRENTUSER:
        this.triggerRefetchUserChanged();
        break;
      case this.API_CHAT_USERS:
        this.fetchUsers();
        break;
      case this.API_CHAT_USERCHATS:
        this.fetchUserChats();
        break;
      case this.API_CHAT_USERMESSAGES:
        this.triggerRefetchRestMessages();
        break;
        
    }

  }

  //############## REALTIME SIGNAL-R ######################
    private realtimeConnection: signalR.HubConnection;

    private realtimeIsActive: number = 0;

    private initialiseRealtimeConnection() {
      /**
       * The server tells, if there is new data, so the REST API can be refetched.
       */
      this.realtimeConnection.on("ChangedNotificationREST" /* Same name in AngularDemoChatSignalR.Server\HubsWebsocket\RealtimeNotificationHubService.cs */, (restEndpointPath: string) => {
        console.log("Triggered ChangedNotificationREST: " + restEndpointPath);
        this.triggerRefetchRest(restEndpointPath.toLowerCase());
      });

      /**
       * Restart SignalR on close
       */
      this.realtimeConnection.onclose(async () => {
        console.log("SigR closed!");
      });

    }

    /**
     * Start the realtime SignalR (websocket e.g.) connection
     */
    private async startRealtimeConnection() {
      //if (this.realtimeIsActive < 1) {
        // No realtime connection active
        //if (this.restIsActive > 0) {
          // Rest currently fetching
          //setTimeout(this.startRealtimeConnection.bind(this), 100);
        //}
        //else {
          // No active Rest fetch
          //this.restIsActive = 0;
          //this.realtimeIsActive++;
          // Start and then register the connection with userId
          this.realtimeConnection.start().then( () => {
            this.realtimeConnectionRegisterUser();
          }
          ).catch((err) => console.error(err));
        //}
      //}
    }

  private stopRealtimeConnection() {
    //this.realtimeConnection.stop().then(() => {
    //  this.realtimeIsActive = 0;
    //}
    //);
     
  }

  /**
   * Register the connection by userId
   */
  private realtimeConnectionRegisterUser() {
    const userId: number | null = this.myChatUserId();
    if (userId != null) {
      this.realtimeConnection.send("registerConnectionUser", userId).then(() => { console.log("SigR: registered user: " + userId) });
    }
    else {
      // Retry when userId is available
      setTimeout(this.realtimeConnectionRegisterUser.bind(this), 100); //todo maybe more event based 
    }
    
  }

  //############## END REALTIME SIGNAL-R ##################


  private restIsActive: number = 0;
  private beforeRest() {
    //this.restIsActive++;
    //if (this.realtimeIsActive > 0) {
    //  this.stopRealtimeConnection();
    //}
  }
  private afterRest() {
    //this.restIsActive--;
  }

  /**
   * CONSTRUCTOR
   * @param httpClient injected
   */
  constructor(httpClient: HttpClient) {
    this.httpClient = httpClient;
    this.users = signal([]);
    this.userChats = signal([]);

    //############## REALTIME SIGNAL-R ##################
    this.realtimeConnection = new signalR.HubConnectionBuilder()
      .withUrl("/sigR/notis", {
        transport: signalR.HttpTransportType.WebSockets
      })
      .configureLogging(signalR.LogLevel.Debug)
      .withAutomaticReconnect()
      .build();
    this.initialiseRealtimeConnection();
    //############## END REALTIME SIGNAL-R ##################
  }


  ngOnInit() {
    
    //Load Users and opened chats on site load
    this.fetchUserChats();
    this.fetchCurrentUser();
    // Fetch users
    this.fetchUsers();
    // Wait till all open fetches are closed and start SigR
    this.startRealtimeConnection();
    
  }

  public openChatOnClk(clickedUser: ChatUser) {
    //select user and open view
    this.currentChatPartner.set(clickedUser)
  }

  public async OnCurrentUserChanged() {
    console.log("UserId: " + this.myChatUserId());
    this.fetchUserChats(this.myChatUserId());
  }

  public async StartChatWithUser(partnerId: number | null) {
    console.log("PartnerId: " + partnerId);
    this.fetchStartChat(partnerId);
  }

  public async scrollToChatMessageEnd(scrollCondition: () => boolean = function (){ return true; }) {
    /* requestAnimationFrame render after angular element updates e.g. chat messages */
    requestAnimationFrame(() => {
      if (scrollCondition()) {
        const element = this.userChatElement.nativeElement;
        element.scrollTop = element.scrollHeight;
      }
    });
  }
  /**
   * Open a new Chat between currentUser and partner (if not already opened)
   * @param partnerId Id of the partner.
   */
  public fetchStartChat(partnerId: number | null) {
    const parameters = {
      partnerId: String(partnerId)
    };
    this.beforeRest();
    this.httpClient.get<ChatUser[]>(this.API_CHAT_OPEN, { params: parameters }).subscribe(
      (result) => {
        console.log('Started chat!');
        this.fetchUserChats(); //Need to refetch chats
        this.afterRest();
      },
      (error) => {
        console.error(error);
        this.afterRest();
      }
    );
  }
  /**
   * Fetch the list of chats related to the current user (authorId or receiverId is user)
   * @param userId set for changing to different user / null --> server knows current user
   */
  public fetchUserChats(userId: number | null = null) {

    let uid;
    if (userId == null) {
      uid = -1;
    }
    else {
      uid = userId;
    }

    const parameters = {
      userId: String(uid)
    };
    this.beforeRest();
    this.httpClient.get<ChatUser[]>(this.API_CHAT_USERCHATS, { params: parameters }).subscribe(
      (result) => {
        this.userChats.set(result);
        console.log('Fetched chats!');
        if (uid != -1) {
          this.fetchCurrentUser(); //After changing the user on server syncronize again
          this.fetchUsers(); //After changing the user on server syncronize again
        }
        this.afterRest();
      },
      (error) => {
        this.afterRest();
        console.error(error);
      }
    );
  }
  
  /** Fetch a list of all Users*/
  public fetchUsers(OnSuccessOrError: () => void = function () { }) {
    this.beforeRest();
    this.httpClient.get<ChatUser[]>(this.API_CHAT_USERS).subscribe(
      (result) => {
        this.users.set(result);
        console.log('Fetched users!');
        OnSuccessOrError();
        this.afterRest();
      },
      (error) => {
        console.error(error);
        OnSuccessOrError();
        this.afterRest();
      }
    );
  }
  
  /** Fetch current user*/
  public fetchCurrentUser() {
    this.beforeRest();
    this.httpClient.get<ChatUser>(this.API_CHAT_CURRENTUSER).subscribe(
      (result) => {
        this.myChatUser.set(result);
        this.myChatUserId.set(result.userId);
        this.realtimeConnectionRegisterUser(); // Server SignalR hub keeps tracking userId for this connection
        this.afterRest();
        console.log('Fetched currentUser!');
      },
      (error) => {
        this.afterRest();
        console.error(error);
      }
    );
  }

  /**
   * Fetch msg for current chat over REST
   * @param settableResult The List for rendering will be set by result.
   * @param partnerId chatPartner (senderId)
   * @param onSucceeded Called if successfully updated (for scrolling)
   */
  public fetchUserMessages(settableResult: WritableSignal<ChatUserMessage[]>, partnerId: number | undefined, onSucceeded: () => void = function (){}) { //todo on sent event 
    if (partnerId != undefined) {

      const parameters = {
        userId: '-1', /* -1 ==> server my currentId */
        partnerId: String(partnerId)
      };
      this.beforeRest();
      this.httpClient.get<ChatUserMessage[]>(this.API_CHAT_USERMESSAGES, { params: parameters }).subscribe(
        (result) => {
          settableResult.set(result);
          onSucceeded();
          this.afterRest();
        },
        (error) => {
          this.afterRest();
          console.error(error);
        }
      );

    }
    else {
      console.error("fetchUserMessages: partnerId undefined!");
    }
  }

  /**
   * Send MSG over REST
   * @param OnReceiveOrError Event on error or result (use IsChatUserState to differ)
   * @param partnerId chatPartner (receiver)
   * @param message msg
   */
  public postUserMessageSend(OnReceiveOrError: (result: ChatUserState | any) => void, partnerId: number, message: string) {
    if (partnerId > -1) {
      const body: ChatUserMessage = {
        created: new Date(Date.now()),
        userId: partnerId,
        message: message
      }
      this.beforeRest();
      this.httpClient.post<ChatUserState>(this.API_CHAT_SENDMESSAGE, body).subscribe(
        (result) => {
          OnReceiveOrError(result);
          this.afterRest();
        },
        (error) => {
          OnReceiveOrError(error);
          this.afterRest();
          console.log(error);
        }
      );
    }
    else {
      console.error("postUserMessageSend: partnerId undefined can't send!");
    }
  }

}

