using JobApplication.Application.DTOs.Auth;
using MediatR;

namespace JobApplication.Application.Features.Auth.Commands.Register;

public sealed record RegisterCommand(RegisterDto Dto) : IRequest<(bool Succeeded, IEnumerable<string> Errors, AuthResponseDto? Response)>;
