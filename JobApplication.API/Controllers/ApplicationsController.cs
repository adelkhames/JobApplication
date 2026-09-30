using JobApplication.Application.Features.Applications.Commands.Apply;
using JobApplication.Application.Features.Applications.Commands.Cancel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JobApplication.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ApplicationsController : ControllerBase
{
    private readonly ISender _sender;

    public ApplicationsController(ISender sender) => _sender = sender;


    [HttpPost]
    [Authorize(Roles = "Candidate")]

    public async Task<IActionResult> Apply([FromBody] ApplyRequest request)
    {
        var appUserId = GetUserId();
        if (appUserId is null)
            return Unauthorized(new { message = "Could not identify the caller." });

        var id = await _sender.Send(new ApplyToJobCommand(request.JobId, appUserId));
        return CreatedAtAction(nameof(Apply), new { id }, new { id });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Candidate")]

    public async Task<IActionResult> Cancel(int id)
    {
        var appUserId = GetUserId();
        if (appUserId is null)
            return Unauthorized(new { message = "Could not identify the caller." });

        await _sender.Send(new CancelApplicationCommand(id, appUserId));
        return NoContent();
    }

    private string? GetUserId()
        => User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
}

public sealed class ApplyRequest
{
    public int JobId { get; set; }
}
