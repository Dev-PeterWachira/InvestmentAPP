using Investment.API.Data;
using Investment.API.Models;
using Investment.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Investment.API.Services
{
    public class FineService : IFineService
    {
        private readonly ApplicationDbContext _context;

        public FineService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task GenerateMonthlyFinesAsync()
        {
            var today = DateTime.UtcNow;

            if (today.Day <= 15)
                return; // Only after the 15th

            var users = await _context.Users.ToListAsync();
            var monthStart = new DateTime(today.Year, today.Month, 1);

            foreach (var user in users)
            {
                var hasContribution = await _context.Contributions
                    .AnyAsync(c => c.UserId == user.Id && c.Date >= monthStart);

                if (hasContribution)
                    continue; // Skip users who paid

                var fineExists = await _context.Fines
                    .AnyAsync(f => f.UserId == user.Id && f.DateIssued >= monthStart);

                if (!fineExists)
                {
                    _context.Fines.Add(new Fine
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        Amount = 500,
                        DateIssued = today
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}