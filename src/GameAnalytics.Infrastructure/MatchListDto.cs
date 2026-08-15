using System.Text.Json.Serialization;

namespace GameAnalytics.Infrastructure
{
    public class MatchListDto
    {
        public required List<MatchItemDto> Data{ get; set; }
    }
    public class MatchItemDto
    {
        public required MatchMetaDto MetaData {  get; set; }

    }

    public class MatchMetaDto
    {
        [JsonPropertyName("match_id")]
        public required string Id { get; set; }
    }


    
}
