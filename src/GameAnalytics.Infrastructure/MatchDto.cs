using System.Text.Json.Serialization;

namespace GameAnalytics.Infrastructure
{
    public class MatchHistoryResponseDto
    {
        public required List<MatchDataDto> Data { get; set; }

    }

    public class SingleMatchResponseDto
    {
        public required MatchDataDto Data { get; set; }
        }

    public class MatchDataDto
    {
        public required MatchMetaData Metadata { get; set; }
        public required List<PlayerDto> Players { get; set; }

        public required List<TeamDto> Teams { get; set; }
        public required List<RoundDto> Rounds { get; set; } 
    }

    public class TeamDto
    {
        [JsonPropertyName("team_id")]
        public required string TeamId { get; set; } 

        public bool Won { get; set; }

        public required TeamRoundsDto Rounds { get; set; }
    }

    public class TeamRoundsDto
    {
        public int Won { get; set; }
        public int Lost { get; set; }
    }
    public class MatchMetaData
    {
        public required Map Map { get; set; }
        [JsonPropertyName("started_at")]
        public required string StartedAt { get; set; }

        public required Queue queue { get; set; }

        [JsonPropertyName("match_id")]
        public required string MatchId { get; set; }
    }

    public class Queue
    {
        public required string Name { get; set; }
    }
    
    public class Map
    {
        public required string Name { get; set; }
    } 

    public class AccountResponseDto 
    { 
        public required AccountData Data { get; set; }
    }

    public class AccountData 
    { 
        public required string Puuid { get; set; }
        [JsonPropertyName("account_level")] 
        public int AccountLevel { get; set; }
        public required string Card { get; set; }
    
    }


}