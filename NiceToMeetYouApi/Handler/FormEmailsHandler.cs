namespace NiceToMeetYouApi.Handler.FormEmails;

public class FormEmail
{
    public class FormEmailRequest
    {
        public required string FirstName { get; set; }
        public required string LastName  { get; set; }
        public required string Domain    { get; set; }
    }

    public class FormEmailResponse
    {
        public List<string>? Emails { get; set; }
    }

    public sealed class FormEmailHandler
    {
        public FormEmailResponse Execute(FormEmailRequest input, CancellationToken ct)
        {
            var firstName = input.FirstName.Trim().ToLowerInvariant();
            var lastName  = input.LastName.Trim().ToLowerInvariant();
            var domain    = input.Domain.Trim().ToLowerInvariant();

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
            foreach (var e in emails)
            {
                if (seen.Add(e)) result.Add(e);
            }

            return new FormEmailResponse
            {
                Emails = result
            };
        }
    }
}
