using MediatR;

namespace JobApplication.Application.Features.Applications.Commands.Apply;

public sealed record ApplyToJobCommand(int JobId, string AppUserId) : IRequest<int>;
