using MediatR;

namespace NiceToMeetYouApi.Handler.FormEmails;

public class FormEmail
{
    public class FormEmailRequest : IRequest<FormEmailResponse>
    {
        public required string FirstName { get; set; }
        public required string LastName  { get; set; }
        public required string Domain    { get; set; }
    }

    public class FormEmailResponse
    {
        public List<string> Emails { get; init; } = new();
    }

    public sealed class FormEmailHandler : IRequestHandler<FormEmailRequest, FormEmailResponse>
    {
        public Task<FormEmailResponse> Handle(FormEmailRequest request, CancellationToken cancellationToken)
        {
            var firstName = request.FirstName.Trim().ToLowerInvariant();
            var lastName  = request.LastName.Trim().ToLowerInvariant();
            var domain    = request.Domain.Trim().ToLowerInvariant();

            var firstInitial = firstName[0].ToString();
            var lastInitial  = lastName[0].ToString();

            var emails = new List<string>(capacity: 10)
            {
                $"{firstName}@{domain}",
                $"{lastName}@{domain}",
                $"{firstName}{lastName}@{domain}",
                $"{firstInitial}{lastName}@{domain}",
                $"{firstName}{lastInitial}@{domain}",
                $"{firstInitial}{lastInitial}@{domain}",
                $"{firstName}.{lastName}@{domain}",
                $"{firstInitial}.{lastName}@{domain}",
                $"{firstName}.{lastInitial}@{domain}",
                $"{lastName}.{firstInitial}@{domain}"
            };

            var seen = new HashSet<string>(StringComparer.Ordinal);
            var result = new List<string>(emails.Count);
            foreach (var email in emails)
            {
                if (seen.Add(email))
                {
                    result.Add(email);
                }
            }

            return Task.FromResult(new FormEmailResponse
            {
                Emails = result
            });
        }
    }
}
