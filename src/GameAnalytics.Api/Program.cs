
using Microsoft.EntityFrameworkCore;
using GameAnalytics.Domain.Services;
using GameAnalytics.Infrastructure;
using GameAnalytics.Application;
using GameAnalytics.Middleware;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddTransient<ExternalApiErrorHandler>();


var apiKey = builder.Configuration["RiotApi:HenrikApiKey"];

builder.Services.AddHttpClient<RiotApiService>(client => 
{
    client.DefaultRequestHeaders.Add("Authorization", apiKey);
})
    .AddHttpMessageHandler<ExternalApiErrorHandler>();

builder.Services.AddScoped<IRiotApiClient>(sp => sp.GetRequiredService<RiotApiService>());

builder.Services.AddScoped<PlayerStatAnalyser>();

builder.Services.AddScoped<IUserRepository, UserRepository>();


var app = builder.Build();

app.UseExceptionHandler();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}






app.MapControllers();

app.Run();


