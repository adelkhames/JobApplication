using MediatR;

namespace JobApplication.Application.Features.Jobs.Commands.CloseJob;

public sealed record CloseJobCommand(int JobId, string RecruiterId) : IRequest<Unit>;
