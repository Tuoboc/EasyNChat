using EasyNChat.Services;
using EasyNChat.WebSocket;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEasyNWsChat();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseEasyNWsChat();
app.UseAuthorization();
app.MapControllers();
app.Run();
