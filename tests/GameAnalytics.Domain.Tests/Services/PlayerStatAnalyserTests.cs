using GameAnalytics.Domain.Entities;
using GameAnalytics.Domain.Services;




public class PlayerStatAnalyserTests
{

    private readonly PlayerStatAnalyser _analyser = new();

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 1)]
    [InlineData(0, 10, 0)]
    [InlineData(20, 5, 4)]
    [InlineData(10, 3, 3.33)]
    public void KillToDeathRatio_ReturnsExpected(int kills, int deaths, double expected)
    {
        var playerStats = new PlayerStats
        {
            Kills = kills,
            Deaths = deaths,
        };
        var result = _analyser.KillToDeathRatio(playerStats);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 1, 1, 2)]
    [InlineData(0, 1, 5, 5)]
    [InlineData(0, 10, 5, 0.5)]
    [InlineData(5, 3, 5, 3.33)]
    public void KillAssistToDeathRatio_ReturnsExpected(int kills, int deaths, int assists, double expected)
    {
        var playerStats = new PlayerStats
            {
                Kills = kills,
                Deaths = deaths,
                Assists = assists
            };
        var result = _analyser.KillAssistToDeathRatio(playerStats);

        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData(20, 0, 0, 100)]
    [InlineData(0, 5, 5, 0)]
    [InlineData(10, 10, 10, 33.3)]
    public void HeadshotPercentage_ReturnsExpected(int headhosts, int bodyshots, int leghosts, double expected)
    {
        var playerStats = new PlayerStats
        {
            Headshots = headhosts,
            Bodyshots = bodyshots,
            Legshots = leghosts
        };
        var result = _analyser.HeadshotPercentage(playerStats);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateMatchStatistics_ReturnsCorrectlyAssembledPerformance()
    {
        var playerStats = new PlayerStats { Kills = 20, Deaths = 5, Assists = 5, Headshots = 10, Bodyshots = 10, Legshots = 10 };

        var result = _analyser.CalculateMatchStatistics(playerStats);

        Assert.Equal(4, result.KdRatio);
        Assert.Equal(5, result.KdaRatio);
        Assert.Equal(33.3, result.HeadshotPercentage);
        Assert.Equal(20, result.Kills);
        Assert.Equal(5, result.Deaths);
        Assert.Equal(5, result.Assists);
    }

}