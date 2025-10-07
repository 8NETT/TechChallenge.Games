namespace TechChallenge.Games.Application.DTOs
{
    public class CommonDTO<T> where T : class
    {
        public T Data { get; set; }
    }
}
