import { Component, ElementRef, NO_ERRORS_SCHEMA, OnInit, ViewChild, WritableSignal, input, signal } from '@angular/core';
import { Chat } from '../../chat.component';
import { ChatUserState, ChatUserState_SUCCEEDED, IsChatUserStateType } from '../../ChatUserStateType';
import { ChatUserView } from '../chat-user-view.component';
/**
 * This defines the form in the footer of the chat with partner view (ChatUserView) to write a response message.
 */
@Component({
  selector: 'chat-reply-form',
  standalone: true,
  templateUrl: 'chat-reply-form.component.html',
  styleUrl: 'chat-reply-form.component.scss'
})
export class ChatReplyForm {

  private chat: Chat;
  private chatUserView: ChatUserView;
  private message: string = "";

  // Needed to clear value
  @ViewChild('ChatMessageTextArea')
  private ChatMessageTextArea!: ElementRef<HTMLTextAreaElement>;

  constructor(chat: Chat, chatUserView: ChatUserView) {
    this.chat = chat;
    this.chatUserView = chatUserView;
  }

  /**
   * Always update the text on tipping.
   * @param tar ChangeEventTarget
   */
  public OnTextChange(tar: EventTarget | null) {
    if (tar != null) {
      const textArea: HTMLTextAreaElement = tar as HTMLTextAreaElement;
      this.message = textArea.value;
    }
    else {
      console.error("ChatReplyForm.OnTextChange(null)!");
    }
  }

  /**
   * Sends the message over the logic in root chat class for to the current (chat-partner).
   */
  public SendMessage() {
    //this.chat.fetchUserMessages(this.messages, this.user()?.userId);
    if (this.message != "") {
      this.chat.postUserMessageSend(this.onSendMessageResult.bind(this) /* --> this != undefined in callback */, this.chat.currentChatPartner()?.userId ?? -1, this.message);
    }
  }

  /**
   * The event for error or success of sending a message.
   * @param result
   */
  public onSendMessageResult(result: ChatUserState | any) {
    // Compare the structure to differ between success and error object
    if (IsChatUserStateType(result)) {
      const cus: ChatUserState = result as ChatUserState;
      if (cus.status == ChatUserState_SUCCEEDED) {
        this.ChatMessageTextArea.nativeElement.value = "";
        this.chatUserView.RefetchUserMessages(); //Trigger refetch messages
      }
      else {
        // Server side error
        console.log("Error on server sending message: " + cus.reason);
      }
    }
    else {
      console.log("Error sending message: " + result);
    }
  }

}
