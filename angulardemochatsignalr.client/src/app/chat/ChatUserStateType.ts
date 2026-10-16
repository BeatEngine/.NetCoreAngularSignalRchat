//This has to be the same structure than AngularDemoChatSignalR.Server\RESTEntities\ChatUserState.cs
export interface ChatUserState {
  dateTime: Date,
  status: string,
  name: string | null | undefined,
  id: number | null | undefined,
  reason: string | null | undefined
}
/** Needs to be implemented in AngularDemoChatSignalR.Server\RESTEntities\ChatUserState.cs as State_SUCCEEDED*/
export const ChatUserState_SUCCEEDED: string = "OK";
/** Needs to be implemented in AngularDemoChatSignalR.Server\RESTEntities\ChatUserState.cs as State_FAIL*/
export const ChatUserState_FAIL: string = "FAIL";
/**
 * Check if object is ChatUserState
 * @param value object
 * @returns is or isn't ChatUserState
 */
export function IsChatUserStateType(value: any) : boolean
{
  return (
    typeof value === "object"
    && value !== null
    && "dateTime" in value
    && "status" in value
  );
}

