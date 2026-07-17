namespace GameAnalytics.Models.External
{
    public class MatchListDto
    {
        public List<MatchItemDto> Data{ get; set; }
    }
    public class MatchItemDto
    {
        public MatchMetaDto Meta {  get; set; }

    }

    public class MatchMetaDto
    {
        public string Id { get; set; }
    }


    
}
