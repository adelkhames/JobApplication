using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.DTOs.Auth;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, (bool Succeeded, IEnumerable<string> Errors, AuthResponseDto? Response)>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<(bool Succeeded, IEnumerable<string> Errors, AuthResponseDto? Response)> Handle(RegisterCommand request, CancellationToken cancellationToken)
        => _identityService.RegisterAsync(request.Dto);
}
