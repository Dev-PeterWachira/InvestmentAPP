using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class DarajaService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _http;

    public DarajaService(IConfiguration config, HttpClient http)
    {
        _config = config;
        _http = http;
    }

    public async Task<string> GetAccessToken()
    {
        var key = _config["Daraja:ConsumerKey"];
        var secret = _config["Daraja:ConsumerSecret"];

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{key}:{secret}")
        );

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", credentials);

        var response = await _http.GetAsync(
            "https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials"
        );

     var content = await response.Content.ReadAsStringAsync();
using var json = JsonDocument.Parse(content);

var token = json.RootElement
                .GetProperty("access_token")
                .GetString();

if (string.IsNullOrEmpty(token))
    throw new Exception("Daraja access token is null or empty");

return token;

    }

    public async Task<string> InitiateStkPush(string phone, decimal amount)
    {
        var token = await GetAccessToken();

        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var shortcode = _config["Daraja:ShortCode"];
        var passkey = _config["Daraja:PassKey"];
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");

        var password = Convert.ToBase64String(
            Encoding.UTF8.GetBytes(shortcode + passkey + timestamp)
        );

        var request = new
        {
            BusinessShortCode = shortcode,
            Password = password,
            Timestamp = timestamp,
            TransactionType = "CustomerPayBillOnline",
            Amount = amount,
            PartyA = phone,
            PartyB = shortcode,
            PhoneNumber = phone,
            CallBackURL = _config["Daraja:CallbackUrl"],
            AccountReference = "Investment",
            TransactionDesc = "Monthly Contribution"
        };

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            Encoding.UTF8,
            "application/json"
        );

        var response = await _http.PostAsync(
            "https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest",
            content
        );

        return await response.Content.ReadAsStringAsync();
    }
}
