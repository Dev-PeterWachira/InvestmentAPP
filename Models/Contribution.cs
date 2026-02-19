using System;
using System.ComponentModel.DataAnnotations;
using Investment.API.Data;

namespace Investment.API.Models
{
    public class Contribution
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]

        public decimal Amount { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        public bool IsLate { get; set; }
        public decimal FineAmount { get; set; }

        public string TransactionId { get; set; } = string.Empty;

        public DateTime Date { get; set; } = DateTime.UtcNow;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
