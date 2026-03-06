using Investment.API.Services.Interfaces;

namespace Investment.API.Services
{
    public class MonthlyFineBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public MonthlyFineBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var fineService = scope.ServiceProvider.GetRequiredService<IFineService>();

                await fineService.GenerateMonthlyFinesAsync();

                // Run once every 24 hours
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}