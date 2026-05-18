using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // וודא שהוספת את זה
using System;
using System.Linq; // עבור Count()
using System.Threading;
using System.Threading.Tasks;
using Service.Interfaces;
using Service.Dto;

namespace Service.Services
{
    public class DailyMatchingService : BackgroundService
    {
        private readonly ILogger<DailyMatchingService> _logger;
        private readonly IServiceScopeFactory _scopeFactory; // הזרקת ה-Factory במקום השירות הישיר

        public DailyMatchingService(
            ILogger<DailyMatchingService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DailyMatchingService is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                var nextRun = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0);

                if (now > nextRun)
                {
                    nextRun = nextRun.AddDays(1);
                }

                var delay = nextRun - now;
                if (delay > TimeSpan.Zero)
                {
                    _logger.LogInformation($"Waiting until next run at {nextRun:yyyy-MM-dd HH:mm:ss}");
                    await Task.Delay(delay, stoppingToken);
                }

                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    _logger.LogInformation("🚀 Starting daily matching algorithm...");

                    // יצירת Scope חדש כדי שנוכל להשתמש בשירות Scoped
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var matchService = scope.ServiceProvider.GetRequiredService<IMatch>();

                        // הרצת האלגוריתם היומי
                        //var matches = await matchService.RunMatchingAlgorithm(0);

                        //_logger.LogInformation($"✅ Daily matching completed. Created/updated {matches.Count} matches");

                        var algorithmMatches = 0;// matches.Count(m => m.IsSelectedByAlgorithm);
                        var totalMatches = 0;// matches.Count;

                        _logger.LogInformation($"📊 Daily Statistics: {algorithmMatches} algorithm matches, {totalMatches} total matches");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error occurred during daily matching");
                }
            }

            _logger.LogInformation("DailyMatchingService is stopping.");
        }
    }

}