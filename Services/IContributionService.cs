using Investment.API.DTOs;
using Investment.API.Models;

namespace Investment.API.Services.Interfaces{
    public interface IContributionService
    {
        Task<Contribution> AddContribution(Guid userId, ContributionDto dto);
        Task<List<Contribution>> GetUserContributions(Guid userId);
        Task<decimal>GetUserTotal(Guid userId);
        Task<decimal> GetSharePercentage(Guid useId);
        
    }

}