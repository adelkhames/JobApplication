namespace JobApplication.Application.Common.Interfaces;

public interface INotificationService
{
    Task NotifyCandidate(int applicationId);
}
