public interface IDarajaService
{
    Task<string> GetAccessToken();
    Task<string> InitiateStkPush(string phone, decimal amount);
}