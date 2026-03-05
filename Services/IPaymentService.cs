using Investment.API.Models;
using System.Text.Json;

namespace Investment.API.Services.Interfaces;

public interface IPaymentService
{
    Task<string> InitiateStkPush(string phone, decimal amount);
    Task<Payment> ProcessMpesaCallback(JsonElement callbackBody);
}