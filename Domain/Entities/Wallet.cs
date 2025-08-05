namespace Domain.Entities
{
    public class Wallet
    {
        public int Id { get; set; }
        public string DocumentId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Movement> Movements { get; set; } = new List<Movement>();
    }
}