namespace TechChallenge.Games.Core.Documents;

public class SearchResult<T> where T : class
{
    public long Total { get; set; }
    public List<T>? Hits { get; set; }
}