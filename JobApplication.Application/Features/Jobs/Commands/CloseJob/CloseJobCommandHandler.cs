using Hangfire;
using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.Exceptions;
using JobApplication.Domain.Entities;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob;

public sealed class CloseJobCommandHandler : IRequestHandler<CloseJobCommand, Unit>
{
    private readonly IJobRepository _jobRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public CloseJobCommandHandler(
        IJobRepository jobRepository,
        IBackgroundJobClient backgroundJobClient)
    {
        _jobRepository = jobRepository;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Unit> Handle(CloseJobCommand request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetByIdAsync(request.JobId)
                  ?? throw new NotFoundException(nameof(Job), request.JobId);

        if (job.RecruiterId != request.RecruiterId)
            throw new ForbiddenException("Only the recruiter who posted this job can close it.");

        if (!job.IsActive && job.ClosedAt.HasValue)
            throw new BusinessRuleException("This job is already closed.");

        job.IsActive = false;
        job.ClosedAt = DateTime.UtcNow;
        job.ClosedBy = request.RecruiterId;

        _jobRepository.Update(job);
        await _jobRepository.SaveChangesAsync();

        // Notify all candidates who applied to this job
        var applications = await _jobRepository.GetApplicationsByJobIdAsync(request.JobId);
        foreach (var application in applications)
        {
            _backgroundJobClient.Enqueue<INotificationService>(
                x => x.NotifyCandidate(application.Id));
        }

        return Unit.Value;
    }
}
