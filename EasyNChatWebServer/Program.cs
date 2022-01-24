using EasyNChat.Services;
using EasyNChat.WebSocket;
using EasyNChatWebServer.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEasyNWsChat<WsChatDemo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseEasyNWsChat<WsChatDemo>();
app.UseAuthorization();
app.MapControllers();
app.Run();
