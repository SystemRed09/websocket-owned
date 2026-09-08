using Microsoft.AspNetCore.SignalR.Client;

namespace Websocket_Client_Echo
{
    class Program
    {
        // Main is async because every modern .NET networking API is async.
        // There is no synchronous alternative.
        static async Task Main(string[] args)
        {
            // Describe a connection to the echo hub on the local machine.
            var hub = new HubConnectionBuilder()
                .WithUrl("http://127.0.0.1:8001/echo")
                .Build();

            // Register what to do when the server invokes "ReceiveMessage" on us.
            hub.On<string>("ReceiveMessage", msg => Console.WriteLine(msg));

            // Open the connection.
            await hub.StartAsync();
            Console.WriteLine("Connected to /echo.");

            Console.Write("Enter a message: ");
            string line = Console.ReadLine();

            // Invoke the Send method on the server's EchoHub.
            await hub.SendAsync("Send", line);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await hub.DisposeAsync();
        }
    }
}
