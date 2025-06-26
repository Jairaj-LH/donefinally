using charac.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class HistoryCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HistoryCleanupService> _logger;

    public HistoryCleanupService(IServiceProvider serviceProvider, ILogger<HistoryCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("🧹 Running HistoryCleanupService at {time}", DateTimeOffset.Now);

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // 🗓️ Set cutoff to 3 days ago (UTC)
                    var cutoffDate = DateTime.UtcNow.AddDays(-3);

                    _logger.LogInformation("🔍 Looking for records older than {cutoff}", cutoffDate);

                    var oldRecords = await db.UserActivityHistories
                        .Where(h => h.Timestamp < cutoffDate)
                        .ToListAsync(stoppingToken);

                    _logger.LogInformation("📦 Found {count} old records", oldRecords.Count);

                    if (oldRecords.Count > 0)
                    {
                        db.UserActivityHistories.RemoveRange(oldRecords);
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("✅ Deleted {count} old records", oldRecords.Count);
                    }
                    else
                    {
                        _logger.LogInformation("📭 No old records found to delete.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Exception during history cleanup.");
            }

            // ⏲️ Run every 24 hours (once per day)
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
