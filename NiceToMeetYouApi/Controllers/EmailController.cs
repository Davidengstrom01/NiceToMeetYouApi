using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiceToMeetYouApi.Handler.FormEmails;

namespace NiceToMeetYouApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmailController(IMediator mediator) : ControllerBase
{
    [HttpPost("form")]
    public async Task<ActionResult<FormEmail.FormEmailResponse>> PostFormEmail(
        [FromBody] FormEmail.FormEmailRequest request,
        CancellationToken ct)
    {
        var emails = await mediator.Send(request, ct);

        

        return Ok(response);
    }
}