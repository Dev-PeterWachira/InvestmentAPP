using Investment.API.Data;
using Investment.API.Models;
using Investment.API.Services.Interfaces;
using System.Text.Json;

namespace Investment.API.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IDarajaService _daraja;
    private readonly ApplicationDbContext _context;

    public PaymentService(IDarajaService daraja, ApplicationDbContext context)
    {
        _daraja = daraja;
        _context = context;
    }

    // ===============================
    // 1️⃣ Initiate STK Push
    // ===============================
    public async Task<string> InitiateStkPush(string phone, decimal amount)
    {
        // Delegates to DarajaService
        var response = await _daraja.InitiateStkPush(phone, amount);
        return response;
    }

    // ===============================
    // 2️⃣ Process Mpesa Callback
    // ===============================
    public async Task<Payment> ProcessMpesaCallback(JsonElement callbackBody)
    {
        var stkCallback = callbackBody
            .GetProperty("Body")
            .GetProperty("stkCallback");

        var resultCode = stkCallback.GetProperty("ResultCode").GetInt32();

        if (resultCode != 0)
        {
            throw new Exception("Payment failed or canceled by user");
        }

        var metadata = stkCallback
            .GetProperty("CallbackMetadata")
            .GetProperty("Item");

        decimal amount = 0;
        string phone = "";
        string receipt = "";

        foreach (var item in metadata.EnumerateArray())
        {
            var name = item.GetProperty("Name").GetString();

            if (name == "Amount")
                amount = item.GetProperty("Value").GetDecimal();

            if (name == "PhoneNumber")
                phone = item.GetProperty("Value").GetInt64().ToString();

            if (name == "MpesaReceiptNumber")
                receipt = item.GetProperty("Value").GetString() ?? "";
        }

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            PhoneNumber = phone,
            Amount = amount,
            MpesaReceiptNumber = receipt,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return payment;
    }
}