using Investment.API.Data;
using Investment.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Investment.API.Services
{
    public class MonthlyFineService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MonthlyFineService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var today = DateTime.UtcNow;

                // Run only after 15th
                if (today.Day > 15)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var users = await context.Users.ToListAsync();

                    foreach (var user in users)
                    {
                        var monthStart = new DateTime(today.Year, today.Month, 1);

                        // Check if user contributed this month
                        var hasContribution = await context.Contributions
                            .AnyAsync(c =>
                                c.UserId == user.Id &&
                                c.Date >= monthStart);

                        if (!hasContribution)
                        {
                            // Check if fine already issued this month
                            var fineExists = await context.Fines
                                .AnyAsync(f =>
                                    f.UserId == user.Id &&
                                    f.DateIssued >= monthStart);

                            if (!fineExists)
                            {
                                context.Fines.Add(new Fine
                                {
                                    Id = Guid.NewGuid(),
                                    UserId = user.Id,
                                    Amount = 500,
                                    DateIssued = today
                                });
                            }
                        }
                    }

                    await context.SaveChangesAsync();
                }

                // Run once every 24 hours
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}
