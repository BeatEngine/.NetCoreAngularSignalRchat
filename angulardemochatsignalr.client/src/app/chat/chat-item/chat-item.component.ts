import { Component, InputSignal, NO_ERRORS_SCHEMA, OnInit, input } from '@angular/core';
import { Chat } from '../chat.component';
import { ChatUser } from '../ChatUserType';
/**
 * Represent a single row of the list with opened user chats.
 */
@Component({
  selector: 'chat-item',
  standalone: true,
  templateUrl: 'chat-item.component.html',
  styleUrl: 'chat-item.component.scss'
})
export class ChatItem {

  private chat: Chat;

  public chatUser: InputSignal<ChatUser> = input.required<ChatUser>();
  constructor(chat: Chat) {
    this.chat = chat;
  }

}
