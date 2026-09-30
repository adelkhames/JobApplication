using JobApplication.Application.DTOs.Auth;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(LoginDto Dto) : IRequest<AuthResponseDto?>;
