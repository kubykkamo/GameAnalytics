
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
    TokenLimit = 30,
    TokensPerPeriod = 30,
    ReplenishmentPeriod = TimeSpan.FromMinutes(1),
    AutoReplenishment = true,
    QueueLimit = 100,
    QueueProcessingOrder = QueueProcessingOrder.OldestFirst
});
builder.Services.AddTransient(sp => new ClientRateLimitingHandler(rateLimiter));

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

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();


