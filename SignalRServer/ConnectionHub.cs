using System.Collections.Concurrent;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;

namespace SignalRServer;

internal class ConnectionHub : Hub
{
  private readonly ConnectionManager _connectionManager;

  public ConnectionHub(ConnectionManager connectionManager)
  {
    _connectionManager = connectionManager;
  }
  
  public override async Task OnConnectedAsync()
  {
    Console.WriteLine($"Client connected: {Context.ConnectionId}");
    var httpContext = Context.GetHttpContext();
    var connectionToken = httpContext!.Request.Query["connection_token"].ToString();

    if (string.IsNullOrEmpty(connectionToken))
    {
      Context.Abort();
    }
    
    _connectionManager.AddConnection(connectionToken, Context.ConnectionId);
    
    var sent =  _connectionManager.GetAllConnections()
      .Where(c => c != Context.ConnectionId)
      .Select(async c => await SendMessageToClient(c, $"New client {Context.ConnectionId} connected."));
    
    await Task.WhenAll(sent);
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    var httpContext = Context.GetHttpContext();
    var connectionToken = httpContext!.Request.Query["connection_token"].ToString();
    Console.WriteLine($"Disconnection started: {connectionToken}: {Context.ConnectionId}");
    
    if (!string.IsNullOrEmpty(connectionToken))
    {
      await Task.Delay(TimeSpan.FromSeconds(10));

      Console.WriteLine($"Remove from client store");
      var currentConnectionId = _connectionManager.GetConnectionId(connectionToken);
      if (currentConnectionId == Context.ConnectionId)
      {
        _connectionManager.RemoveConnection(connectionToken);
        Console.WriteLine($"Client with token {connectionToken} disconnected: {Context.ConnectionId}");
      }
    }

    await base.OnDisconnectedAsync(exception);
  }
  
  public Task UpdateConnectionId(string connectionToken)
  {
    _connectionManager.UpdateConnection(connectionToken, Context.ConnectionId);
    Console.WriteLine($"Client reconnected: {Context.ConnectionId}");
    return Task.CompletedTask;
  }
  
  public Task ReceiveEvent(Message message)
  {
    var consoleMessage = $"Event {message.MessageBody} has been processed from token {message.ConnectionToken}.";
    
    Console.WriteLine($"Received event with Id: {message.MessageBody} from client {Context.ConnectionId}");
    Clients.Caller.SendAsync("ReceiveMessage", consoleMessage);
    
    return Task.CompletedTask;
  }
    
  public Task BroadcastMessage(string message)
  {
    Console.WriteLine($"Broadcasting message: {message}");
    return Clients.All.SendAsync("ReceiveMessage", message);
  }
    
  public Task SendMessageToClient(string connectionId, string message)
  {
    Console.WriteLine($"Sending message to client {connectionId}: {message}");
    return Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
  }
}

internal record Message(string ConnectionToken, string MessageBody);