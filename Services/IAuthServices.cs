using Investment.API.DTOs;
using Investment.API.Models;

namespace Investment.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}