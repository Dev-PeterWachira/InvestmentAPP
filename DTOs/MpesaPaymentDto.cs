namespace Investment.API.Dtos
{
    public class MpesaPaymentDto
    {
        public decimal Amount { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
