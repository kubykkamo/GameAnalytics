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
        var json = await File.ReadAllTextAsync("testdata/account_success.json");
        SetFalseResponse(HttpStatusCode.OK, json);

        var result = await _service.GetPlayerId("Player", "1234");

        Assert.Equal("id-12345", result);
    }

    [Fact]
    public async Task GetUserId_ShouldReturn404_WhenUserDoesNotExist()
    {
        var json = await File.ReadAllTextAsync("testdata/account_not_found.json");

        SetFalseResponse(HttpStatusCode.OK, json);

        
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPlayerId("Unknown", "0000"));
    }

    [Fact]
    public async Task GetMatches_ShouldReturnMatchIds_WhenMatchesExist()
    {
    
        var json = """
        {
            "data": [
                { "metadata": { "match_id": "cc23acfd-8a87-47bd-a144-e51f0b8b4352" } },
                { "metadata": { "match_id": "f403a966-9942-436e-ab5e-ee695f94be2a" } },
                { "metadata": { "match_id": "782b3ad3-d730-403a-8c79-b3b857483301" } },
                { "metadata": { "match_id": "532b2c40-8c15-41ec-bb09-ec8a8ccc6549" } },
                { "metadata": { "match_id": "0cbec596-fad6-4cf7-8abf-5c51095a577a" } }
            ]
        }
        """;

        SetFalseResponse(HttpStatusCode.OK, json);

  
        var result = await _service.GetMatches("Player", "1234");

        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
        Assert.Equal("cc23acfd-8a87-47bd-a144-e51f0b8b4352", result[0]);
        Assert.Equal("f403a966-9942-436e-ab5e-ee695f94be2a", result[1]);
        Assert.Equal("782b3ad3-d730-403a-8c79-b3b857483301", result[2]);
        Assert.Equal("532b2c40-8c15-41ec-bb09-ec8a8ccc6549", result[3]);
        Assert.Equal("0cbec596-fad6-4cf7-8abf-5c51095a577a", result[4]);
    }

    [Fact]
    public async Task GetAccountInfo_NullData_ThrowsNotFoundException()
    {
        var json = """
        {
            "data": null
        }
        """;
        
        SetFalseResponse(HttpStatusCode.OK, json);

        await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.GetAccountInfo("Player", "1234"));
    }

    
    [Fact]
    public async Task GetMatches_ShouldThrowInvalidOperationException_WhenDataIsNull()
    {
        var json = """
        {
            "data": null
        }
        """;

        SetFalseResponse(HttpStatusCode.OK, json);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetMatches("Unknown", "0000"));
    }
    

    [Fact]
    public async Task GetMatchDetails_ValidMatch_ReturnsMappedMatchDetails()
    {
        
        var json = await File.ReadAllTextAsync("testdata/match_details_success.json");
        SetFalseResponse(HttpStatusCode.OK, json);

        
        var result = await _service.GetMatchDetails("match_id123");

        
        Assert.NotNull(result);
        
        Assert.Equal("match_id123", result.MatchId); 
        Assert.NotEmpty(result.Players);
        
        
        var firstPlayer = result.Players.First();
        Assert.NotNull(firstPlayer.Stats);
        Assert.True(firstPlayer.Stats.Kills >= 0);
    }

    [Fact]
    public async Task GetMatchDetails_NullData_NotFoundException()
    {
        
        var json = await File.ReadAllTextAsync("testdata/match_details_not_found.json");
        SetFalseResponse(HttpStatusCode.OK, json);

        
        await Assert.ThrowsAsync<NotFoundException>(() => 
            _service.GetMatchDetails("match_id123"));
    }
}