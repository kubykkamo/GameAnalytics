using GameAnalytics.Data;
using System;
using GameAnalytics.Models.External;
using GameAnalytics.Services;
using Microsoft.EntityFrameworkCore;
using GameAnalytics.Models.Internal;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<RiotApiService>();
builder.Services.AddScoped<PlayerStatAnalyser>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var analyser = new GameAnalytics.Models.Internal.PlayerStatAnalyser();
var testData = new PlayerStatsDto { Headshots = 20, Bodyshots = 30, Legshots = 20, Assists = 8, Kills = 15, Deaths = 10 };
var result = analyser.HeadshotPercentage(testData);
var kd = analyser.KillToDeathRatio(testData);
var kad = analyser.KillAssistToDeathRatio(testData);


Console.WriteLine(result);
Console.WriteLine(kd);
Console.WriteLine(kad); 
Console.ReadLine();



app.MapControllers();

app.Run();


