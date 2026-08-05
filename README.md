# MessageService

Subscription-based network messaging for [Mirror](https://github.com/MirrorNetworking/Mirror), built for Unity **2022.3.62f3**.

Mirror disconnects a client that receives a `NetworkMessage` type with no registered handler. This project adds a subscription layer on top of Mirror (without modifying Mirror source) so the server only sends a message type to clients that explicitly subscribed to it.

## Solution overview

| Piece | Role |
|-------|------|
| `IClientMessageService` / `ClientMessageService` | Client: register local handler, then notify server |
| `IServerMessageService` / `ServerMessageService` | Server: track subscribers, send only to them |
| `SubscribeRequest` / `UnsubscribeRequest` | Client → server subscription protocol |
| `MessagingNetworkManager` | Exposes Mirror lifecycle as C# events for DI services |
| `GameLifetimeScope` | VContainer composition root |
| `HelloMessage*Demo` | Assignment demo flow |

**Safety guarantee:** `Subscribe<T>` always registers the Mirror handler *before* sending `SubscribeRequest`, so the client never receives an unhandled message type.

## Project layout

```
MessageService/                 ← Unity project
  Assets/MessageService/        ← solution code (messages, networking, DI, demo)
  Assets/Mirror/                ← Mirror networking plugin
  Assets/Scenes/SampleScene.unity
  Packages/jp.hadashikick.vcontainer/  ← VContainer 1.19.0 (local package)
```

## Requirements

- Unity 2022.3.62f3
- Mirror (included)
- VContainer 1.19.0 (included under `Packages/`)

## How to run

1. Open `MessageService/` in Unity 2022.3.62f3.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Enter Play mode.
4. Click **Host (Server + Client)** in the Network Manager HUD.

Expected Console output:

```
[Server] Connection 0 subscribed to HelloMessage, greeting it.
[Client] Received HelloMessage: Hello Client!
```

## Scenario covered

1. Server / host starts  
2. Client connects  
3. Client subscribes to `HelloMessage`  
4. Server receives the subscription  
5. Server sends `HelloMessage` with text `"Hello Client!"`  
6. Client logs the text to the Console  
