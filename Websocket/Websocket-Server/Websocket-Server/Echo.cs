using Microsoft.AspNetCore.SignalR;

namespace Websocket_Server
{
    /// <summary>
    /// The warm-up example. A client sends a message; the server sends it
    /// straight back to that same client and to nobody else.
    ///
    /// This is already complete - you do not need to change it. Read it to see
    /// the shape of a Hub before you start on ChatHub.
    /// </summary>
    public class EchoHub : Hub
    {
        /// <summary>
        /// Invoked by the client with:  connection.SendAsync("Send", text)
        ///
        /// The old lab overrode OnMessage() and every message went through it.
        /// With SignalR you instead declare a public method per operation, and
        /// the client calls it by name. The method name is the contract.
        /// </summary>
        public async Task Send(string message)
        {
            // Clients.Caller = only the connection that invoked this method.
            // "ReceiveMessage" is the name of the method being invoked back on
            // the client. The client must register a handler under that name.
            await Clients.Caller.SendAsync("ReceiveMessage", "Echo: " + message);
        }
    }
}
