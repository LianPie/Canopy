using Canopy.Models;
using Canopy.Repositories;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Canopy.Services
{
    public class TaskReminderService : ITaskReminderService
    {
        private readonly ITasksRepository _taskRepo;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TaskReminderService> _logger;

        public TaskReminderService(
            ITasksRepository taskRepo,
            INotificationService notificationService,
            ILogger<TaskReminderService> logger)
        {
            _taskRepo = taskRepo;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task SendDailyRemindersAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var allUsers = _taskRepo.GetAllUserIdsWithTasks(); 

                foreach (var userId in allUsers)
                {
                    await SendReminderForUserAsync(userId, today);
                }

                _logger.LogInformation("Daily reminders sent for {Date}", today);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send daily reminders");
                throw;
            }
        }

        private async Task SendReminderForUserAsync(int userId, DateTime date)
        {
            // Get all tasks for this user on this date
            var tasks = new List<PlannedTask>();

            // 1. Tasks with specific date
            var datedTasks = _taskRepo.GetByDate(userId, date);
            tasks.AddRange(datedTasks);

            // 2. Tasks without a date (check-in style)
            var undatedTasks = _taskRepo.GetWithoutDate(userId);
            tasks.AddRange(undatedTasks);

            // 3. Recurring tasks that fall on this date
            var recurringTasks = _taskRepo.Getreaccuring(userId);
            var activeRecurring = recurringTasks
                .Where(t => IsRecurringActiveToday(t, date))
                .ToList();
            tasks.AddRange(activeRecurring);

            if (!tasks.Any())
            {
                // Optionally send a "No tasks today" check-in reminder
                await SendCheckInReminderAsync(userId, date);
                return;
            }

            var payload = new
            {
                date = date.ToString("yyyy-MM-dd"),
                taskCount = tasks.Count,
                tasks = tasks.Select(t => new
                {
                    t.Id,
                    t.Title,
                    t.Description,
                    t.DeadLine,
                    t.Recurrence, // Daily, Weekly, Monthly
                    isRecurring = t.Recurrence != RecurrenceType.None
                }),
                summary = $"You have {tasks.Count} task(s) for today"
            };

            var jsonPayload = JsonSerializer.Serialize(payload);

            // Send notification 
            await _notificationService.SendAsync(
                userId,
                NotificationType.DailyReminder, 
                jsonPayload
            );

            _logger.LogDebug("Sent daily reminder to user {UserId}: {TaskCount} tasks",
                userId, tasks.Count);
        }

        private bool IsRecurringActiveToday(PlannedTask task, DateTime date)
        {
            if (task.Recurrence == RecurrenceType.None) return false;

            switch (task.Recurrence)
            {
                case RecurrenceType.Daily:
                    // Check if the task should run daily
                    return !task.IsRecurrenceEnded || date >= task.DeadLine.Value.Date;

                case RecurrenceType.Weekly:
                    // Check if it's the correct day of week
                    if (!string.IsNullOrEmpty(task.RecurrenceWeekday))
                    {
                        return date.DayOfWeek.ToString() == task.RecurrenceWeekday;
                    }
                    return false;

                case RecurrenceType.Monthly:
                    // Check if it's the correct day of month
                    if (task.RecurrenceMonthDay.HasValue)
                    {
                        return date.Day == task.RecurrenceMonthDay.Value;
                    }
                    return false;

                default:
                    return false;
            }
        }

        private async Task SendCheckInReminderAsync(int userId, DateTime date)
        {
            // Send a simple "check-in" reminder even if no tasks
            var payload = new
            {
                type = "check-in",
                date = date.ToString("yyyy-MM-dd"),
                message = "Good morning! No tasks due today, but don't forget to check in!"
            };

            await _notificationService.SendAsync(
                userId,
                NotificationType.DailyCheckIn,
                JsonSerializer.Serialize(payload)
            );
        }
    }
}