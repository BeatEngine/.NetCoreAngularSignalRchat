import { Component, input, InputSignal, OnInit, WritableSignal } from '@angular/core';
import { Chat } from '../chat.component';
import { ChatUser } from '../ChatUserType';

/**
 * Head of the Window with all opened user chats. (Not the chat with partner view.)
 */

@Component({
  selector: 'chat-head',
  standalone: true,
  templateUrl: 'chat-head.component.html',
  styleUrl: 'chat-head.component.scss'
})
export class ChatHead {
  private chat: Chat;

  public users: InputSignal<ChatUser[]> = input.required<ChatUser[]>();
  public myUser: WritableSignal<ChatUser>;
  constructor(chat: Chat) {
    this.chat = chat;
    this.myUser = chat.myChatUser;
;
  }
  /**
   * Event Sets current user to the selected and reloads all opened chats.
   * @param tar HTML-EventTarget
   */
  public OnSelectUser(tar: EventTarget | null) {
    const select = tar as HTMLSelectElement;
    const idx: number = select.selectedIndex;
    const val: string = select.options[idx].value;

    this.chat.myChatUserId.set(Number(val));
    this.chat.OnCurrentUserChanged(); //Triggers reload of current chats
    
  }

  /**
   * For starting a new chat when partner selected.
   * @param tar HTML-EventTarget
   */
  public OnSelectPartner(tar: EventTarget | null) {
    const select = tar as HTMLSelectElement;
    const idx: number = select.selectedIndex;
    const val: string = select.options[idx].value;

    this.chat.StartChatWithUser(Number(val)); //Also triggers reload of current chats

  }

}
