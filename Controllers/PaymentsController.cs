using Investment.API.Data;
using Investment.API.Models;
using Investment.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Investment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IDarajaService _darajaService;
        private readonly ApplicationDbContext _context;

        public PaymentsController(IDarajaService darajaService, ApplicationDbContext context)
        {
            _darajaService = darajaService;
            _context = context;
        }

        // ===============================
        // 1️⃣ Initiate STK Push
        // ===============================
        [HttpPost("stkpush")]
        public async Task<IActionResult> StkPush(string phoneNumber, decimal amount)
        {
            try
            {
                var response = await _darajaService.InitiateStkPush(phoneNumber, amount);

                return Ok(new
                {
                    message = "STK Push initiated. Check phone.",
                    mpesaResponse = response
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Failed to initiate payment",
                    error = ex.Message
                });
            }
        }

        // ===============================
        // 2️⃣ Mpesa Callback
        // ===============================
        [HttpPost("callback")]
        public async Task<IActionResult> MpesaCallback([FromBody] JsonElement body)
        {
            try
            {
                var stkCallback = body
                    .GetProperty("Body")
                    .GetProperty("stkCallback");

                var resultCode = stkCallback.GetProperty("ResultCode").GetInt32();

                if (resultCode != 0)
                {
                    return Ok(new
                    {
                        message = "Payment failed"
                    });
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

                return Ok(new
                {
                    message = "Payment recorded successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error processing callback",
                    error = ex.Message
                });
            }
        }
    }
}