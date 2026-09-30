using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CreateJob;

public sealed record CreateJobCommand(
    string Title,
    string Description,
    bool IsActive,
    string RecruiterId) : IRequest<int>;
