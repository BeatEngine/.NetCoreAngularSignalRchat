//This has to be the same structure than AngularDemoChatSignalR.Server\RESTEntities\ChatUserMessage.cs
export interface ChatUserMessage {
  created: Date;
  userId: number;
  message: string;
}
