using JobApplication.Application.DTOs.Auth;

namespace JobApplication.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, IEnumerable<string> Errors, AuthResponseDto? Response)> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);
}
