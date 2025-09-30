namespace TechChallenge.Games.Application.DTOs
{
    public class SearchRequest
    {
        public string Termo { get; set; } = string.Empty;
        public int From { get; set; } = 0;
        public int Size { get; set; } = 10;
    }
}
