using Websocket_Server;

// Build the web host that will carry our SignalR hubs.
var builder = WebApplication.CreateBuilder(args);

// Register the SignalR services with the dependency injection container.
builder.Services.AddSignalR();

var app = builder.Build();

// Map the Echo hub to the /echo endpoint  (was: wss.AddWebSocketService<Echo>("/echo"))
app.MapHub<EchoHub>("/echo");

// Map the Chat hub to the /chat endpoint  (was: wss.AddWebSocketService<Chat>("/chat"))
app.MapHub<ChatHub>("/chat");

Console.WriteLine("Server listening on http://127.0.0.1:8001");
Console.WriteLine("  Echo hub -> /echo");
Console.WriteLine("  Chat hub -> /chat");
Console.WriteLine("Press Ctrl+C to exit.");

// Start listening. This call blocks until the server is shut down.
app.Run("http://127.0.0.1:8001");
