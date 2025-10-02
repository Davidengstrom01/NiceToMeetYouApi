namespace NiceToMeetYouApi.Models.FormEmails;

public class FormEmailRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Domain { get; set; }
}
