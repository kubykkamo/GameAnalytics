
using Microsoft.EntityFrameworkCore;
using GameAnalytics.Domain.Services;
using GameAnalytics.Infrastructure;
using GameAnalytics.Application;
using GameAnalytics.Middleware;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddTransient<ExternalApiErrorHandler>();

var rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
{
    TokenLimit = 15,
    TokensPerPeriod = 15,
    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
    AutoReplenishment = true,
    QueueLimit = 100,
    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
});

builder.Services.AddTransient(sp => new ClientRateLimitingHandler(rateLimiter, sp.GetRequiredService<ILogger<ClientRateLimitingHandler>>()));

var apiKey = builder.Configuration["HenrikApi:ApiKey"];
builder.Services.AddHttpClient<IRiotApiClient, RiotApiService>(client => 
{
    client.DefaultRequestHeaders.Add("Authorization", apiKey);
    client.BaseAddress = new Uri("https://api.henrikdev.xyz");
})
.AddHttpMessageHandler<ExternalApiErrorHandler>()
.AddHttpMessageHandler<ClientRateLimitingHandler>();

builder.Services.AddScoped<PlayerStatAnalyser>();
builder.Services.AddScoped<MatchAnalysisService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


var app = builder.Build();

var klic = builder.Configuration["HenrikApi:ApiKey"]; 

app.Logger.LogInformation("STARTUP CHECK - Klic v cloudu: Nacteno={IsLoaded}, Delka={Length}", 
    !string.IsNullOrEmpty(klic), 
    klic?.Length ?? 0);

app.UseExceptionHandler();


app.UseSwagger();
app.UseSwaggerUI();


app.MapControllers();

app.Run();


