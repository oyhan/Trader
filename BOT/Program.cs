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

builder.Services.ConfigureDependencies(builder.Configuration);
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();


var telegramService = builder.Services.BuildServiceProvider().GetRequiredService<TelegramService>();

#if DEBUG
var period = 50000;

#else   
var period = 5000;
#endif
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
        period: period);


app.Run();

