using Investment.API.Models;

namespace Investment.API.Services.Interfaces
{
    public interface IFineService
    {
        Task GenerateMonthlyFinesAsync();
    }
}