using JobApplication.Application.Common.Interfaces;
using JobApplication.Application.DTOs.Auth;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JobApplication.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public IdentityService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IConfiguration configuration,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors, AuthResponseDto? Response)> RegisterAsync(RegisterDto dto)
    {
        if (dto.Role != "Recruiter" && dto.Role != "Candidate")
            return (false, new[] { "Role must be 'Recruiter' or 'Candidate'." }, null);

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            Name = dto.Name,
            Role = dto.Role
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description), null);

        if (dto.Role == "Candidate")
        {
            _context.Candidates.Add(new Candidate
            {
                Name = dto.Name,
                Email = dto.Email,
                AppUserId = user.Id,
                CvUrl = string.Empty
            });
            await _context.SaveChangesAsync();
        }

        return (true, Array.Empty<string>(), CreateAuthResponse(user));
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        return result.Succeeded ? CreateAuthResponse(user) : null;
    }

    private AuthResponseDto CreateAuthResponse(AppUser user)
        => new()
        {
            Token = GenerateJwtToken(user),
            Email = user.Email!,
            Role = user.Role
        };

    private string GenerateJwtToken(AppUser user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var mins) ? mins : 60;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, user.Role),
            new("role", user.Role),
            new("name", user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
