
using Investment.API.Data;
namespace Investment.API.Models
{
    public class Fine
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public decimal Amount { get; set; } = 500;

        public int Month { get; set; }
        public int Year { get; set; }

        public bool IsPaid { get; set; } = false;

        public DateTime DateIssued { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
