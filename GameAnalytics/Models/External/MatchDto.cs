using System.Text.Json.Serialization;

namespace GameAnalytics.Models.External
{
    public class MatchHistoryResponseDto
    {
        public List<MatchDataDto> Data { get; set; }

    }

    public class SingleMatchResponseDto
    {
        public MatchDataDto Data { get; set; }
        }

    public class MatchDataDto
    {
        public MatchMetaData Metadata { get; set; }
        public List<PlayerDto> Players { get; set; }
        public List<RoundDto> Rounds { get; set; } 
    }

    public class MatchMetaData
    {
        public Map Map { get; set; }
        [JsonPropertyName("started_at")]
        public string StartedAt { get; set; }

        public Queue queue { get; set; }
    }

    public class Queue
    {
        public string Name { get; set; }
    }
    
    public class Map
    {
        public string Name { get; set; }
    } 

    public class PlayerDto
    {
        public string Puuid { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }

        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        public Agent Agent { get; set; }

        public PlayerStatsDto Stats { get; set; }

        public PlayerPerformanceDto Performance { get; set; }

    }

    public class Agent
    {
        public string Name { get; set; }
    }

    public class PlayerStatsDto
    {
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public int Headshots { get; set; }
        public int Bodyshots { get; set; }

        public int Legshots { get; set; }

    }

    public class RoundDto
    {
        public int Id { get; set; }
        public string Result { get; set; }

        [JsonPropertyName("winning_team")]
        public string Winner {  get; set; }
       
    }

    public class RoundPlayerStatsDto
    {
        public PlayerInfoDto Player { get; set; }
        public List<DamageEventDto> DamageEvents { get; set; }
    }

    public class PlayerInfoDto
    {
        public string Puuid { get; set; }
        public string Name { get; set; }

    }

    public class DamageEventDto
    {
        public PlayerInfoDto Player { get; set; }
        public int Headshots { get; set; }
        public int Bodyshots { get; set; }
        public int Legshots { get; set; }
        public int Damage { get; set; }
    }



     public class AccountResponseDto 
    { 
        public AccountData Data { get; set; }
    }

    public class AccountData 
    { 
        public string Puuid { get; set; }
        [JsonPropertyName("account_level")] 
        public int AccountLevel { get; set; }
        public string Card { get; set; }
    
    }

    public class PlayerPerformanceDto
    {
        public double headshotPercentage { get; set; }
        public double kdRatio { get; set; }
        public double kdaRatio { get; set; }
        public double kills {  get; set; }
        public double deaths { get; set; }
        public double assists { get; set; }
    }
    
}