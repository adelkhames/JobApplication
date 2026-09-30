using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Features.Auth.Commands.Login;
using JobApplication.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _sender.Send(new RegisterCommand(dto));

        if (!result.Succeeded)
        {
            if (result.Errors.Contains("Role must be 'Recruiter' or 'Candidate'."))
                return BadRequest(new { message = result.Errors.First() });

            return BadRequest(new { errors = result.Errors });
        }

        return Ok(result.Response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var response = await _sender.Send(new LoginCommand(dto));
        if (response is null)
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(response);
    }
}
