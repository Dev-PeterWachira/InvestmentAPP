namespace Investment.API.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        public string PhoneNumber { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string MpesaReceiptNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}