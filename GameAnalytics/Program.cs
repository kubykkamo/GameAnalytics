using GameAnalytics.Data;
using System;
using GameAnalytics.Models.External;
using GameAnalytics.Services;
using Microsoft.EntityFrameworkCore;
using GameAnalytics.Models.Internal;
using GameAnalytics.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHttpClient<RiotApiService>();
builder.Services.AddScoped<PlayerStatAnalyser>();

var app = builder.Build();

app.UseExceptionHandler();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}






app.MapControllers();

app.Run();


