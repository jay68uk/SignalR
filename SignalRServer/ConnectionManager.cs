using System.Collections.Concurrent;

namespace SignalRServer;

public class ConnectionManager
{
  private readonly ConcurrentDictionary<string, string> _connections = new();

  public void AddConnection(string connectionToken, string connectionId)
  {
    _connections.TryAdd(connectionToken, connectionId);
  }

  public void RemoveConnection(string connectionToken)
  {
    _connections.TryRemove(connectionToken, out _);
  }
  
  public string GetConnectionId(string connectionToken)
  {
    return (_connections.GetValueOrDefault(connectionToken)) ?? string.Empty;
  }


  public void UpdateConnection(string connectionToken, string connectionId)
  {
    _connections[connectionToken] = connectionId;
  }

  public IEnumerable<string> GetAllConnections()
  {
    return _connections.Values;
  }
}