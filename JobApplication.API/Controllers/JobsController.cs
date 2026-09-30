using JobApplication.Application.Features.Jobs.Commands.CloseJob;
using JobApplication.Application.Features.Jobs.Commands.CreateJob;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JobApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobsController : ControllerBase
{
    private readonly ISender _sender;

    public JobsController(ISender sender) => _sender = sender;

    [HttpPost]
    [Authorize(Roles = "Recruiter")]

    public async Task<IActionResult> Create([FromBody] CreateJobCommandRequest request)
    {
        var recruiterId = GetUserId();
        if (recruiterId is null)
            return Unauthorized(new { message = "Could not identify the caller." });

        var id = await _sender.Send(new CreateJobCommand(
            request.Title,
            request.Description,
            request.IsActive,
            recruiterId));

        return Ok(new { id });
    }

    [HttpPut("{id:int}/close")]
    [Authorize(Roles = "Recruiter")]

    public async Task<IActionResult> Close(int id)
    {
        var recruiterId = GetUserId();
        if (recruiterId is null)
            return Unauthorized(new { message = "Could not identify the caller." });

        await _sender.Send(new CloseJobCommand(id, recruiterId));
        return NoContent();
    }

    private string? GetUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
}

public sealed class CreateJobCommandRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
