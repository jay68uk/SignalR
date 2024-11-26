using Microsoft.AspNetCore.SignalR.Client;

namespace SignalRApp.Services;

public class SignalRService : IAsyncDisposable
{
  private readonly HubConnection _hubConnection;
  private readonly string ConnectionToken = Guid.NewGuid().ToString();
  public event Action<string> OnMessageReceived;

  public SignalRService()
  {
    _hubConnection = new HubConnectionBuilder()
      .WithUrl($"https://localhost:7193/connection?connection_token={ConnectionToken}")
      .WithAutomaticReconnect()
      .Build();

    _hubConnection.On<string>("ReceiveMessage", (message) =>
    {
      Console.WriteLine($"Received message from server: {message}");
      OnMessageReceived?.Invoke(message);
    });
    
    _hubConnection.Reconnected += async (connectionId) =>
    {
      await UpdateConnectionIdOnServer();
    };
    
    _hubConnection.Closed += (error) =>
    {
      Console.WriteLine(error == null
        ? "Connection closed without error."
        : $"Connection closed due to an error: {error}");
      
      return Task.CompletedTask;
    };
  }

  public async Task StartAsync()
  {
    await _hubConnection.StartAsync();
    Console.WriteLine("Connection started");
  }
  
  public async Task SendEvent(string id)
  {
    var message = new Message(ConnectionToken, id);
    await _hubConnection.InvokeAsync("ReceiveEvent", message);
  }
  
  public async Task InvokeServerMethod(string methodName, params object[] args)
  {
    await _hubConnection.InvokeAsync(methodName, args);
  }

  private async Task UpdateConnectionIdOnServer()
  {
    await _hubConnection.InvokeAsync("UpdateConnectionId", ConnectionToken);
  }
  
  public async ValueTask DisposeAsync()
  {
    await _hubConnection.DisposeAsync();
  }
}

internal record Message(string ConnectionToken, string MessageBody);