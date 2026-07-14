namespace GameAnalytics.Models.External
{
    public class MatchResponseDto
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

        public PlayerStatsDto Stats { get; set; }

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
}