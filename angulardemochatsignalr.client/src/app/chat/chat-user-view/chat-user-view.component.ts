import { Component, ElementRef, ViewChild, WritableSignal, signal } from '@angular/core';
import { Chat } from '../chat.component';
import { ChatUserMessage } from '../ChatUserMessageType';
import { ChatUser } from '../ChatUserType';
import { ChatReplyForm } from './chat-reply-form/chat-reply-form.component';
/**
 *
 * This view shows all the messages the user has with the current chatpartner and a from to reply.
 *
 *ChatUserView
  {
  1_____________
   ++ChatReplyForm
  }
 */
@Component({
  selector: 'chat-user-view',
  imports: [ChatReplyForm],
  standalone: true,
  templateUrl: 'chat-user-view.component.html',
  styleUrl: 'chat-user-view.component.scss'
})
export class ChatUserView {

  private chat: Chat;

  public user: WritableSignal<ChatUser | null>;

  public GetPartnerName() {
    return this.user()?.userName ?? "null";
  }

  public messages: WritableSignal<ChatUserMessage[]>;

  private initUserId: number | undefined;

  // Needed for the needToScroll condition (when the chatMessageList overlaps the reply-form)
  @ViewChild('chatMessageList')
  private chatMessageList!: ElementRef<HTMLDivElement>;
  @ViewChild('chatReplyContainer')
  private chatReplyContainer!: ElementRef<HTMLDivElement>;

  /**
   * Needs to scroll (true) if the list is more hight than the space of the container!
   * Chat overlaps with reply form --> scroll to end
   */
  public needToScrollChatMessages(): boolean {
    const listBottom: number = this.chatMessageList.nativeElement.getBoundingClientRect().bottom;
    const ReplyFormTop: number = this.chatReplyContainer.nativeElement.getBoundingClientRect().top;
    return listBottom+10 > ReplyFormTop; // Chat collides with reply form --> scroll to end
  }
  
  /**
   * Returns the class for the message depending on (is my or from partner)
   * chat-message-li + chat-message-li-my-msg or chat-message-li-partner-msg
   * @param authorId For compare to myId 
   * @returns chat-message-li and ( chat-message-li-my-msg or chat-message-li-partner-msg )
   */
  protected getClassDependingAuthor(authorId: number):string {
    let classes = "chat-message-li";

    if (authorId == this.initUserId) {
      classes += " chat-message-li-my-msg";
    }
    else {
      classes += " chat-message-li-partner-msg";
    }

    return classes as string;
  }
  constructor(chat: Chat) {
    this.chat = chat;
    this.user = chat.currentChatPartner;

    //For speed optimisations load on construction (be carefull no funtions like direct chat-change one to other are intiutivly possible so).
    this.initUserId = chat.currentChatPartner()?.userId;

    //Initially fetch all messages in this chat
    this.messages = signal([]);
    this.RefetchUserMessages();
  }

  /**
   * Event for refetching the user messages over the chat root-class.
   * With scroll-condition.
   */
  public RefetchUserMessages() {
    this.chat.fetchUserMessages(this.messages, this.user()?.userId, /*Scroll chat end*/ this.afterRefetchUserMessages.bind(this) /* this = ChatUserView */ );
  }

  /**
   * Event when the messages has updated for scrolling.
   */
  public afterRefetchUserMessages() {
      this.chat.scrollToChatMessageEnd(this.needToScrollChatMessages.bind(this)/* this = ChatUserView */);
  }


  /**
   * Close chat with partner view 
   */
  public closeCurrentUserChat() {
    this.messages.set([]);
    this.user.set(null);
    this.chat.OnCurrentUserChanged(); // Reload users for preview msg
  }

}
