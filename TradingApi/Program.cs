using Belem.Core;
using Belem.Core.Services;
using System.Configuration;
using TradingApi.Controllers.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.ConfigureDependencies(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await ApplicationLogger.Log(TimeZone.CurrentTimeZone.StandardName);
await ApplicationLogger.Log("*****VERSION 3.0.0*******");

app.UseTelegramLogger();
//await app.UseTelegramClient();
await app.SetTimers();



app.Run();
