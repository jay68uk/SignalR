using SignalRServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
  options.AddDefaultPolicy(policy =>
  {
    policy.WithOrigins("https://localhost:7070")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
  });
});

builder.Services.AddSingleton<ConnectionManager>();

builder.Services.AddHostedService<EventWorker>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.MapHub<ConnectionHub>("/connection");

app.Run();