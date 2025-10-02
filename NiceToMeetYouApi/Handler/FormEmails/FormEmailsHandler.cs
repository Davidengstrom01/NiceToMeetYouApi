using NiceToMeetYouApi.Models.FormEmails;

namespace NiceToMeetYouApi.Handler.FormEmails;

public interface IFormEmailsHandler
{
    FormEmailResponse Execute(FormEmailRequest input, CancellationToken ct);
}

public sealed class AlgorithmHandler : IFormEmailsHandler
{
    public FormEmailResponse Execute(FormEmailRequest input, CancellationToken ct)
    {
        var emails = new List<string>();

        var firstName = input.FirstName.Trim().ToLower();
        var lastName = input.LastName.Trim().ToLower();
        var domain = input.Domain.Trim().ToLower();

        string firstInitial = firstName.Substring(0, 1);
        string lastInitial = lastName.Substring(0, 1);

        emails.Add($"{firstName}@{domain}");
        emails.Add($"{lastName}@{domain}");
        emails.Add($"{firstName}{lastName}@{domain}");
        emails.Add($"{firstInitial}{lastName}@{domain}");
        emails.Add($"{firstName}{lastInitial}@{domain}");
        emails.Add($"{firstInitial}{lastInitial}@{domain}");
        emails.Add($"{firstName}.{lastName}@{domain}");
        emails.Add($"{firstInitial}.{lastName}@{domain}");
        emails.Add($"{firstName}.{lastInitial}@{domain}");
        emails.Add($"{lastName}.{firstInitial}@{domain}");

        return new FormEmailResponse
        {
            Emails = new HashSet<string>(emails).ToList() 
        };
    }
}