using Investment.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Investment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ===============================
        // 1️⃣ Initiate STK Push
        // ===============================
        [HttpPost("stkpush")]
        public async Task<IActionResult> StkPush(string phoneNumber, decimal amount)
        {
            try
            {
                var response = await _paymentService.InitiateStkPush(phoneNumber, amount);

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
                var payment = await _paymentService.ProcessMpesaCallback(body);

                return Ok(new
                {
                    message = "Payment recorded successfully",
                    payment
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