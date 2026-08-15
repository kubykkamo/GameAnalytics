using System.Text.Json.Serialization;

namespace GameAnalytics.Infrastructure
{
    public class MatchListDto
    {
        public List<MatchItemDto> Data{ get; set; }
    }
    public class MatchItemDto
    {
        public MatchMetaDto MetaData {  get; set; }

    }

    public class MatchMetaDto
    {
        [JsonPropertyName("match_id")]
        public string Id { get; set; }
    }


    
}
