using FastEndpoints;
using NiceToMeetYouApi.Handler.FormEmails;
using NiceToMeetYouApi.Models.FormEmails;

namespace NiceToMeetYouApi.Controllers;

public sealed class ComputeEndpoint(IFormEmailsHandler handler)
    : Endpoint<FormEmailRequest, FormEmailResponse>
{
    public override void Configure() => Post("/Email/Form");

    public override Task HandleAsync(FormEmailRequest req, CancellationToken ct)
    {
        var response = handler.Execute(req, ct);


    }
}
