using Investment.API.Data;
using Investment.API.DTOs;
using Investment.API.Models;    
using Investment.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Investment.API.Services
{
    public class ContributionService : IContributionService
    {
        private readonly ApplicationDbContext _context;
        public ContributionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Contribution> AddContribution(Guid userId, ContributionDto dto)
        {
            if(dto.Amount < 2000)
            {
                throw new Exception("Minimum contribution is 2000");

            }
            var today = DateTime.UtcNow;
            bool isLate = today.Day > 15;
            decimal fine = isLate ? 500 : 0;

            var contribution = new Contribution
            {
                UserId = userId,
                Amount = dto.Amount,
                Date = today,
                IsLate = isLate,
                FineAmount = fine
            };

            _context.Contributions.Add(contribution);
            await _context.SaveChangesAsync();
            return contribution;
        }

        public async Task<List<Contribution>>GetUserContributions(Guid userId)
        {
            return await _context.Contributions
                 .Where(c => c.UserId == userId)
                 .ToListAsync();
        }
        public async Task<decimal> GetUserTotal(Guid userId)
        {
            return await _context.Contributions
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Amount);

        }
        public async Task<decimal> GetSharePercentage(Guid userId)
        {
            var totalPool = await _context.Contributions.SumAsync(c=> c.Amount);
            if(totalPool == 0)
            return 0;

            var userTotal = await _context.Contributions
            .Where(c => c.UserId == userId)
            .SumAsync(c => c.Amount);

            return (userTotal / totalPool) * 100;

        }
    }
}