using System.Text.Json.Serialization;

namespace GameAnalytics.Infrastructure
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

        public List<TeamDto> Teams { get; set; }
        public List<RoundDto> Rounds { get; set; } 
    }

    public class TeamDto
    {
        [JsonPropertyName("team_id")]
        public string TeamId { get; set; } 

        public bool Won { get; set; }

        public TeamRoundsDto Rounds { get; set; }
    }

    public class TeamRoundsDto
    {
        public int Won { get; set; }
        public int Lost { get; set; }
    }
    public class MatchMetaData
    {
        public Map Map { get; set; }
        [JsonPropertyName("started_at")]
        public string StartedAt { get; set; }

        public Queue queue { get; set; }

        [JsonPropertyName("match_id")]
        public string MatchId { get; set; }
    }

    public class Queue
    {
        public string Name { get; set; }
    }
    
    public class Map
    {
        public string Name { get; set; }
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


}