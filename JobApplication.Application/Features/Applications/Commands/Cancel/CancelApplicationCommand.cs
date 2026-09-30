using MediatR;

namespace JobApplication.Application.Features.Applications.Commands.Cancel;

public sealed record CancelApplicationCommand(int ApplicationId, string AppUserId) : IRequest<Unit>;
