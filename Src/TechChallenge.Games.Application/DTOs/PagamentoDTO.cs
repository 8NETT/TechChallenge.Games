namespace TechChallenge.Games.Application.DTOs
{
    public class PagamentoDTO
    {
        public int OrderId { get; init; }
        public int UserId { get; init; }
        public Guid JogoId { get; init; }
        public decimal Amount { get; init; }
        public int Status { get; init; }
    }
}
