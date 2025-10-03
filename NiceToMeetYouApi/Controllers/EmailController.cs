using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiceToMeetYouApi.Handler.FormEmails;
using NiceToMeetYouApi.Handler.ValidateEmails;

namespace NiceToMeetYouApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class EmailController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ValidateEmail.ValidateEmailResponse>> Post(
        [FromBody] FormEmail.FormEmailRequest request,
        CancellationToken ct)
    {
        var formResponse = await mediator.Send(request, ct);

        var emails = formResponse.Emails;

        if (emails.Count == 0)
        {
            return Ok(new ValidateEmail.ValidateEmailResponse
            {
                Emails = emails
            });
        }

        var validationResponse = await mediator.Send(new ValidateEmail.ValidateEmailRequest
        {
            Emails = emails
        }, ct);

        return Ok(validationResponse);
    }
}
