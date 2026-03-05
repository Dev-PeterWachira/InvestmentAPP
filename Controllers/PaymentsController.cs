using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Investment.API.Dtos;


namespace Investment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly DarajaService _daraja;

        public PaymentsController(DarajaService daraja)
        {
            _daraja = daraja;
        }

        // ============================
        // 1. Initiate STK Push
        // ============================
        [Authorize]
        [HttpPost("stk")]
        public async Task<IActionResult> StkPush(MpesaPaymentDto dto)
        {
            if (dto.Amount <= 0)
                return BadRequest("Amount must be greater than zero");

            var response = await _daraja.InitiateStkPush(dto.PhoneNumber, dto.Amount);

            return Ok(response);
        }

        // ============================
        // 2. Daraja Callback
        // ============================
        [AllowAnonymous]
        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] JsonElement data)
        {
            var body = data
                .GetProperty("Body")
                .GetProperty("stkCallback");

            var resultCode = body.GetProperty("ResultCode").GetInt32();

            if (resultCode == 0)
            {
                var metadata = body
                    .GetProperty("CallbackMetadata")
                    .GetProperty("Item");

                decimal amount = 0;
                string phone = "";

                foreach (var item in metadata.EnumerateArray())
                {
                    var name = item.GetProperty("Name").GetString();

                    if (name == "Amount")
                        amount = item.GetProperty("Value").GetDecimal();

                    if (name == "PhoneNumber")
                        phone = item.GetProperty("Value").GetInt64().ToString();
                }

                // TODO: Save payment to database
                Console.WriteLine($"Payment received: {phone} - {amount}");
            }

            return Ok();
        }
    }
}
