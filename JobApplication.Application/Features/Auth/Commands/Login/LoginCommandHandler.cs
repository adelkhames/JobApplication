using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.DTOs.Auth;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto?>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public Task<AuthResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
        => _identityService.LoginAsync(request.Dto);
}
