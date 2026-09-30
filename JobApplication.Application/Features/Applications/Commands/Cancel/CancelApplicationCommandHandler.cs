using Hangfire;
using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.Exceptions;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.Applications.Commands.Cancel;

public sealed class CancelApplicationCommandHandler : IRequestHandler<CancelApplicationCommand, Unit>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public CancelApplicationCommandHandler(
        IApplicationRepository applicationRepository,
        IBackgroundJobClient backgroundJobClient)
    {
        _applicationRepository = applicationRepository;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Unit> Handle(CancelApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdAsync(request.ApplicationId)
                          ?? throw new NotFoundException(nameof(JobCandidateApplication), request.ApplicationId);

        var candidate = await _applicationRepository.GetCandidateByAppUserIdAsync(request.AppUserId)
                        ?? throw new NotFoundException(nameof(Candidate), request.AppUserId);

        if (application.CandidateId != candidate.Id)
            throw new ForbiddenException("You can only cancel your own applications.");

        if (application.JobApplicationStatus is JobApplicationStatus.Interview
            or JobApplicationStatus.Accepted
            or JobApplicationStatus.Rejected)
        {
            throw new BusinessRuleException(
                $"Cannot cancel an application with status '{application.JobApplicationStatus}'. " +
                "Only Applied or UnderReview applications can be cancelled.");
        }

        var now = DateTime.UtcNow;
        application.JobApplicationStatus = JobApplicationStatus.Cancelled;
        application.CancelledAt = now;
        application.StatusUpdatedAt = now;

        _applicationRepository.Update(application);
        await _applicationRepository.SaveChangesAsync();

        // Enqueue background notification after saving
        _backgroundJobClient.Enqueue<INotificationService>(x => x.NotifyCandidate(request.ApplicationId));

        return Unit.Value;
    }
}
