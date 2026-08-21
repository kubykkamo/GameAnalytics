using Moq;
using Moq.Protected;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using GameAnalytics.Domain.Exceptions;
namespace GameAnalytics.Infrastructure.Tests.Services;
public class RiotApiServiceTests
{
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly HttpClient _httpClient;
    private readonly RiotApiService _service;
    
    public RiotApiServiceTests()
    {
  
        _handlerMock = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_handlerMock.Object);

        var logger = NullLogger<RiotApiService>.Instance;

        _service = new RiotApiService(_httpClient, logger);
    }
    
    private void SetFalseResponse(HttpStatusCode statusCode, string jsonContent)
    {
    _handlerMock
        .Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
        )
        .ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent(jsonContent)
        });
    }

    [Fact]
    public async Task GetUserId_ShouldReturnUserId_WhenUserExists()
    {
        var json = """
        {
            "data": {
                "puuid": "id-12345"
            }
        }
        """;
        SetFalseResponse(HttpStatusCode.OK, json);

        var result = await _service.GetPlayerId("Player", "1234");

        Assert.Equal("id-12345", result);
    }

    [Fact]
    public async Task GetUserId_ShouldReturn404_WhenUserDoesNotExist()
    {
        var json = """
        {
            "status": 404,
            "message": "Player not found"
        }
        """;

        SetFalseResponse(HttpStatusCode.NotFound, json);

        
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPlayerId("Unknown", "0000"));
    }

    [Fact]
    public async Task GetAccountInfo_ValidData_ReturnsAccountInfo()
    {
        
        var json = """
        {
            "data": {
                "puuid": "id-12345",
                "account_level": 150,
                "card": "some-card-id"
            }
        }
        """;
        
        SetFalseResponse(HttpStatusCode.OK, json);
             
        var result = await _service.GetAccountInfo("Player", "1234");
 
        Assert.NotNull(result);
        Assert.Equal("id-12345", result.Puuid);
        Assert.Equal(150, result.AccountLevel);
        Assert.Equal("some-card-id", result.Card);
    }

    [Fact]
    public async Task GetAccountInfo_NullData_ThrowsInvalidOperationException()
    {
        var json = """
        {
            "data": null
        }
        """;
        
        SetFalseResponse(HttpStatusCode.OK, json);

        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.GetAccountInfo("Player", "1234"));
    }
}