using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.Exceptions;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.Applications.Commands.Apply;

public sealed class ApplyToJobCommandHandler : IRequestHandler<ApplyToJobCommand, int>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IJobRepository _jobRepository;

    public ApplyToJobCommandHandler(
        IApplicationRepository applicationRepository,
        IJobRepository jobRepository)
    {
        _applicationRepository = applicationRepository;
        _jobRepository = jobRepository;
    }

    public async Task<int> Handle(ApplyToJobCommand request, CancellationToken cancellationToken)
    {
        var candidate = await _applicationRepository.GetCandidateByAppUserIdAsync(request.AppUserId)
                        ?? throw new NotFoundException(nameof(Candidate), request.AppUserId);

        var job = await _jobRepository.GetByIdAsync(request.JobId)
                  ?? throw new NotFoundException(nameof(Job), request.JobId);

        if (!job.IsActive)
            throw new BusinessRuleException("Cannot apply to a job that is not active.");

        if (await _applicationRepository.ExistsAsync(request.JobId, candidate.Id))
            throw new ConflictException("You have already applied to this job.");

        var now = DateTime.UtcNow;
        var application = new JobCandidateApplication
        {
            JobId = request.JobId,
            CandidateId = candidate.Id,
            JobApplicationStatus = JobApplicationStatus.Applied,
            AppliedAt = now,
            StatusUpdatedAt = now
        };

        await _applicationRepository.InsertAsync(application);
        await _applicationRepository.SaveChangesAsync();

        return application.Id;
    }
}
