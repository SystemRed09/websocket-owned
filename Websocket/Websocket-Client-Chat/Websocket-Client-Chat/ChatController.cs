using Microsoft.AspNetCore.SignalR.Client;

namespace Websocket_Client_Chat
{
    /// <summary>
    /// The Controller. It owns the connection to the server and knows nothing
    /// about Windows Forms. The View reaches it only through the MessageEntered
    /// method and the MessageReceived event.
    /// </summary>
    class ChatController
    {
        private readonly string name;
        private readonly HubConnection hub;

        // Event raised when a message arrives from the server.
        public event Message MessageReceived;

        public ChatController(string name)
        {
            this.name = name;

            // Build the connection. Note this does NOT connect yet - it only
            // describes the connection. (was: new WebSocket("ws://...") )
            hub = new HubConnectionBuilder()
                .WithUrl("http://127.0.0.1:8001/chat")
                .Build();

            // Register a handler for the "ReceiveMessage" method that the
            // server invokes on us. The name must match what the Hub sends.
            // (was: ws.OnMessage += (sender, e) => ... e.Data ... )
            hub.On<string>("ReceiveMessage", msg => MessageReceived?.Invoke(msg));
        }

        /// <summary>
        /// Opens the connection.
        ///
        /// This is separate from the constructor on purpose. Connecting causes
        /// the server's OnConnectedAsync to fire and the message history to
        /// arrive immediately - and the View cannot display anything until its
        /// window handle exists. Program.cs therefore calls this from the
        /// form's Shown event, not before.
        /// </summary>
        public async Task ConnectAsync()
        {
            await hub.StartAsync();
        }

        /// <summary>
        /// Handles a new message typed by the user. Returns false if we are not
        /// connected, which tells the View to leave the text box alone.
        /// </summary>
        public bool MessageEntered(string message)
        {
            if (hub.State != HubConnectionState.Connected)
            {
                return false;
            }

            // Invoke the SendMessage method on the server's ChatHub.
            // (was: ws.Send(name + ": " + message); )
            // The discard sends without waiting, so the UI thread never blocks.
            _ = hub.SendAsync("SendMessage", name + ": " + message);
            return true;
        }

        /// <summary>
        /// Closes the connection. Called from Program.cs when the form closes.
        /// (The old code used a finalizer, which is not a reliable place to
        /// release a network connection.)
        /// </summary>
        public async Task DisconnectAsync()
        {
            await hub.DisposeAsync();
        }
    }
}
