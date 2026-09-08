using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace Websocket_Server
{
    // ========================================================================
    // Task I:  Modify this hub so that it remembers all messages.
    //          Whenever a new client connects, send all stored messages to
    //          that client only, so it knows the history of the conversation.
    //
    // Task II: Give each message a sequential timestamp (1, 2, 3, ...) at the
    //          moment it is stored, and keep that number attached to the
    //          message forever.
    //
    // ------------------------------------------------------------------------
    // HINT 1 (read this one twice):
    //   A Hub instance is created and destroyed FOR EVERY METHOD CALL. Not per
    //   client - per call. If you store the history in an ordinary instance
    //   field, it will be gone before the next message arrives.
    //   There is a modifier keyword that makes a field belong to the class
    //   itself rather than to any one instance. You need it.
    //
    // HINT 2:
    //   Because every connected client can be invoking SendMessage at the same
    //   moment on different threads, the shared collection must be thread-safe.
    //   Look at System.Collections.Concurrent. Choose carefully: the rubric
    //   requires messages to come back IN ORDER, and not every concurrent
    //   collection preserves order.
    //
    // HINT 3:
    //   A plain  counter++  is not atomic and can lose increments under
    //   concurrency. See System.Threading.Interlocked.
    //
    // HINT 4:
    //   Sending to everybody and sending to one client are different targets:
    //       Clients.All     -> every connected client
    //       Clients.Caller  -> only the client whose call you are handling
    //                          (inside OnConnectedAsync, that is the client
    //                           that just connected)
    //       Clients.Client(Context.ConnectionId) -> the same thing, spelled out
    //                          using the connection's unique ID
    // ========================================================================
    public class ChatHub : Hub
    {
        // TODO (Task I):  Declare the shared, thread-safe message history here.
        private ConcurrentQueue<string> history = new();
        // TODO (Task II): Declare the shared sequential counter here.
        private int counter = 0;


        /// <summary>
        /// Called automatically by SignalR when a new client connects.
        /// (This replaces the old OnOpen() from WebSocketBehavior.)
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            // TODO (Task I): Send every stored message to the client that just
            //                connected - and to nobody else. Do NOT broadcast.

            await base.OnConnectedAsync();
            {
                foreach (string msg in history)
                {
                    await Clients.Caller.SendAsync("ReceiveMessage", msg);
                }
            }
        }

        private readonly object Gate = new();
        /// <summary>
        /// Invoked by a client with:  connection.SendAsync("SendMessage", text)
        /// (This replaces the old OnMessage(MessageEventArgs e).)
        /// </summary>
        public async Task SendMessage(string message)
        {
            // TODO (Task II): Assign the next sequential number to this message.
            // TODO (Task I):  Store the numbered message in the shared history.
            lock (Gate)
            {
                history.Enqueue(counter + ": " + message);
                counter++;
            }
            

            // Broadcast the message to all clients.
            // (was: Sessions.Broadcast(msg);)
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
