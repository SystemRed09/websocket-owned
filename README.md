# Lab — Real-Time Chat with SignalR

A three-project solution demonstrating real-time client–server communication in C#.

| Project | What it is |
|---|---|
| `Websocket-Server` | ASP.NET Core host with two SignalR hubs: `/echo` and `/chat` |
| `Websocket-Client-Echo` | Console client — the warm-up example, already complete |
| `Websocket-Client-Chat` | WinForms chat client — Model/View/Controller split, already complete |

Your work happens in **one file**: `Websocket-Server/Websocket-Server/Chat.cs`.

---

## Requirements

- Visual Studio 2026 (18.0 or later)
- The **.NET desktop development** and **ASP.NET and web development** workloads
- .NET 10 SDK (installed by those workloads)

Open `Lab-Websockets.sln` at the repository root. All three projects load together.

---

## Running it

You need the server plus at least two chat clients running at the same time.

**1. Configure multiple startup projects (once):**

Right-click the **solution** in Solution Explorer → **Configure Startup Projects…** → **Multiple startup projects** → set `Websocket-Server` and `Websocket-Client-Chat` to **Start**. Press OK.

**2. Press F5.** The server console appears and one chat window opens.

**3. Launch a second client.** Right-click `Websocket-Client-Chat` → **Debug** → **Start New Instance**. Repeat for a third if you like.

Each client asks for a user name, then joins the conversation.

To try the echo warm-up instead, start `Websocket-Server` and then start a new instance of `Websocket-Client-Echo`.

---

## How the pieces talk

The server never calls the client directly and the client never calls the server directly. Both invoke **named methods** on each other across the connection.

```
Client                                     Server (ChatHub)
------                                     ----------------
hub.SendAsync("SendMessage", text)  ---->  public Task SendMessage(string message)

hub.On<string>("ReceiveMessage",…)  <----  Clients.All.SendAsync("ReceiveMessage", msg)
                                           Clients.Caller.SendAsync("ReceiveMessage", msg)
```

If you rename a method on one side, you must rename the matching string on the other. There is no compiler check across that boundary — a typo produces silence, not an error. This is the single most common way to lose an hour on this lab.

---

## Your tasks

See the comment block at the top of `Chat.cs`. In short:

1. **Task I** — the server remembers every message, and a newly connected client receives the full history (that client only, not a broadcast).
2. **Task II** — every message carries a sequential number (1, 2, 3, …) assigned when it is stored.

---

## Submission

Push your completed code to your GitHub repository, and update this `README.md` with a section explaining your approach and any challenges you hit.
