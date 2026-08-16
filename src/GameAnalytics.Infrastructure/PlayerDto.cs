using System.Text.Json.Serialization;
using GameAnalytics.Domain.Entities;
namespace GameAnalytics.Infrastructure
{
    public class PlayerDto
    {
        public string Puuid { get; set; }
        public string Name { get; set; }
        public string Tag { get; set; }

        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        public Agent Agent { get; set; }

        public PlayerStatsDto Stats { get; set; }



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
        public int Score { get; set; }

    }
// TODO: wire up for H2H feature
    public class RoundDto
    {
        public int Id { get; set; }
        public string Result { get; set; }

        [JsonPropertyName("winning_team")]
        public string Winner { get; set; }

    }
// TODO: wire up for H2H feature
    public class RoundPlayerStatsDto
    {
        public PlayerInfoDto Player { get; set; }
        public List<DamageEventDto> DamageEvents { get; set; }
    }
// TODO: wire up for H2H feature
    public class PlayerInfoDto
    {
        public string Puuid { get; set; }
        public string Name { get; set; }

        public string Tag { get; set; }

        public string Team { get; set; }

    }
// TODO: wire up for H2H feature
    public class DamageEventDto
    {
        public PlayerInfoDto Player { get; set; }
        public int Headshots { get; set; }
        public int Bodyshots { get; set; }
        public int Legshots { get; set; }
        public int Damage { get; set; }
    }
}
