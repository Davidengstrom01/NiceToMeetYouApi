namespace NiceToMeetYouApi.Handler.FormEmails;

public class ValidateEmail
{
    public class ValidateEmailRequest
    {
        public required List<string> Emails { get; set; }
    }

    public class ValidateEmailResponse
    {
        public List<string>? Emails { get; set; }
    }

    public sealed class ValidateEmailsHandler
    {
        public ValidateEmailResponse Execute(ValidateEmailRequest input, CancellationToken ct)
        {
            var validEmails = new List<string>();
            var invalidEmails = new List<string>();

            foreach (var email in input.Emails)
            {
                if (IsValidEmail(email))
                {
                    validEmails.Add(email);
                }
                else
                {
                    invalidEmails.Add(email);
                }
            }

            return new ValidateEmailResponse
            {
                Emails = validEmails
            };
        }

        private bool IsValidEmail(string email)
        {
            // Basic email validation logic
            return email.Contains("@") && email.Contains(".");
        }
    }
}