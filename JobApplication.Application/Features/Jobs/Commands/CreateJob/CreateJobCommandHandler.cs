using JobApplication.Application.Common.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CreateJob;

public sealed class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, int>
{
    private readonly IJobRepository _jobRepository;

    public CreateJobCommandHandler(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<int> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var job = new Job
        {
            Title = request.Title,
            Description = request.Description,
            IsActive = request.IsActive,
            RecruiterId = request.RecruiterId
        };

        await _jobRepository.InsertAsync(job);
        await _jobRepository.SaveChangesAsync();

        return job.Id;
    }
}
