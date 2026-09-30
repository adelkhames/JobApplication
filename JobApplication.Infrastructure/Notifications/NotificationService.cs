using JobApplication.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobApplication.Infrastructure.Notifications;

public sealed class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task NotifyCandidate(int applicationId)
    {
        // Simulate sending an email / push notification to the candidate.
        // Replace with real email/SMS logic when ready.
        _logger.LogInformation(
            "[NotificationService] Candidate notification sent for ApplicationId={ApplicationId} at {UtcNow}",
            applicationId,
            DateTime.UtcNow);

        return Task.CompletedTask;
    }
}
