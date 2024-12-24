# SignalR Demo

Comprised of a server, background worker and client application.

The server application outputs console log of all the events received.

The background worker generates an event every few seconds.

The client allows for an input and will show a streaming list of all events received.

This demonstrates simple SignalR connections and how client and server events can be handled.

## Running
1. Open a terminal and navigate to SignalRServer folder - cd .\SignalRServer
2. Start the server(this also starts the background service) dotnet run --launch-profile "https" 
3. Console log outputs will be shown in this terminal.
4. Open a terminal and navigate to SignalRServer folder - cd .\SignalRServer
5. Start the client dotnet run --launch-profile "https"
6. Once started open a browser window using the https link.
7. ![img.png](img.png)
8. Open multiple browser windows with that same link, these can also be incognito windows as well.
9. Adding data to the text box will show it being received in the Server console log.
10. Closing one of the client browser windows will show a disconnection event.