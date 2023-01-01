using Belem.Core;
using Belem.Core.Services;
using System;
using System.Text.RegularExpressions;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var appSettings = new AppSettings();
 builder.Configuration.GetSection("AppSettings").Bind(appSettings);


builder.Services.AddSingleton(appSettings);
builder.Services.AddSingleton(new TelegramBotClient(appSettings.BotApiKey));
builder.Services.AddSingleton<TelegramService>();
builder.Services.AddSingleton<ImageProcessor>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


var telegramService = builder.Services.BuildServiceProvider().GetRequiredService<TelegramService>();
var timer = new Timer(async (state) =>
{
    try
    {
        await telegramService.CheckMessages();
    }
    catch (Exception ex)
    {

    }
},
state: null,
        dueTime: 0,
        period: 5000);


app.Run();

