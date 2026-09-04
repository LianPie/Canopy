using NCrontab;
using System.Diagnostics;

namespace Canopy.Services
{
    public class DailyReminderBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyReminderBackgroundService> _logger;
        private readonly CrontabSchedule _schedule;
        private DateTime _nextRun;

        public DailyReminderBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<DailyReminderBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            // Run at 9 AM every day (adjust as needed)
            // For testing, you might want: "0 * * * *" (every hour)
            // Or "*/5 * * * *" (every 5 minutes)
            _schedule = CrontabSchedule.Parse("0 9 * * *");
            _nextRun = _schedule.GetNextOccurrence(DateTime.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;

                if (now >= _nextRun)
                {
                    try
                    {
                        _logger.LogInformation("Starting daily reminder job");

                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var reminderService = scope.ServiceProvider
                                .GetRequiredService<ITaskReminderService>();

                            await reminderService.SendDailyRemindersAsync();
                        }

                        _logger.LogInformation("Daily reminder job completed");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Daily reminder job failed");
                    }
                    finally
                    {
                        // Schedule the next run
                        _nextRun = _schedule.GetNextOccurrence(now);
                    }
                }

                // Check every minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}