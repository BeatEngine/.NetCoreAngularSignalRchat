import { Component, OnInit } from '@angular/core';
import { Chat } from './chat/chat.component'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  standalone: false,
  styleUrl: './app.component.css'
})
  // The actual root logic code begins in chat.component.ts
export class AppComponent implements OnInit {

  constructor() {} 

  ngOnInit() {

  }


  title = 'angulardemochatsignalr.client';
}
