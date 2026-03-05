namespace Investment.API.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        public string PhoneNumber { get; set; }

        public decimal Amount { get; set; }

        public string MpesaReceiptNumber { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}