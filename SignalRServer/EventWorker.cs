using Microsoft.AspNetCore.SignalR;

namespace SignalRServer;

internal class EventWorker : BackgroundService
{
  private readonly IHubContext<ConnectionHub> _hubContext;
  private readonly ConnectionManager _connectionManager;

  public EventWorker(IHubContext<ConnectionHub> hubContext, ConnectionManager connectionManager)
  {
    _hubContext = hubContext;
    _connectionManager = connectionManager;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      var message = "Background event at " + DateTime.Now.ToString("T");
      Console.WriteLine(message);
      
      await _hubContext.Clients.All.SendAsync("ReceiveMessage", message, cancellationToken: stoppingToken);

      await Task.Delay(5000, stoppingToken);
    }
  }
}