using Microsoft.AspNetCore.SignalR;

namespace MessengerApp.Hubs;

public class ChatHub : Hub
{
    public async Task Send(string message)
    {
        await Clients.All.SendAsync("Receive", message, Context.ConnectionId);
    }
    
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} has joined chat");
        
        var context = Context.GetHttpContext();

        if (context is not null)
        {
            if (context.Request.Cookies.ContainsKey("name"))
            {
                if (context.Request.Cookies.TryGetValue("name", out var userName))
                {
                    Console.WriteLine($"name = {userName}");
                }
            }

            Console.WriteLine($"UserAgent = {context.Request.Headers["User-Agent"]}");

            Console.WriteLine($"RemoteIpAddress = {context.Connection.RemoteIpAddress}");
        }

        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.All.SendAsync("Notify", $"{Context.ConnectionId} has disconnected");
        
        await base.OnDisconnectedAsync(exception);
    }
}