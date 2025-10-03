using DnsClient;
using MediatR;
using System.Net.Mail;
using System.Net.Sockets;

namespace NiceToMeetYouApi.Handler.ValidateEmails;

public class ValidateEmail
{
    public class ValidateEmailRequest : IRequest<ValidateEmailResponse>
    {
        public required List<string> Emails { get; set; }
    }

    public class ValidateEmailResponse
    {
        public List<string> Emails { get; init; } = new();
    }

    public sealed class ValidateEmailsHandler : IRequestHandler<ValidateEmailRequest, ValidateEmailResponse>
    {
        private static readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(5);
        private readonly LookupClient _lookupClient;

        public ValidateEmailsHandler()
        {
            _lookupClient = new LookupClient();
        }

        public async Task<ValidateEmailResponse> Handle(ValidateEmailRequest request, CancellationToken cancellationToken)
        {
            var validEmails = new List<string>();
            var processed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var email in request.Emails)
            {
                if (!processed.Add(email))
                {
                    continue;
                }

                if (!IsValidEmailFormat(email, out var domain))
                {
                    continue;
                }

                if (await CanConnectToEmailDomainAsync(domain, cancellationToken).ConfigureAwait(false))
                {
                    validEmails.Add(email);
                }
            }

            return new ValidateEmailResponse
            {
                Emails = validEmails
            };
        }

        private static bool IsValidEmailFormat(string email, out string domain)
        {
            domain = string.Empty;

            try
            {
                var mailAddress = new MailAddress(email);
                domain = mailAddress.Host;
                return !string.IsNullOrWhiteSpace(domain);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private async Task<bool> CanConnectToEmailDomainAsync(string domain, CancellationToken cancellationToken)
        {
            var hosts = await ResolveMailHostsAsync(domain, cancellationToken).ConfigureAwait(false);

            foreach (var host in hosts)
            {
                if (await TryConnectAsync(host, cancellationToken).ConfigureAwait(false))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task<IReadOnlyList<string>> ResolveMailHostsAsync(string domain, CancellationToken cancellationToken)
        {
            try
            {
                var queryResult = await _lookupClient
                    .QueryAsync(domain, QueryType.MX, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                var hosts = queryResult.Answers
                    .MxRecords()
                    .OrderBy(r => r.Preference)
                    .Select(r => r.Exchange.Value.TrimEnd('.'))
                    .Where(h => !string.IsNullOrWhiteSpace(h))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (hosts.Count > 0)
                {
                    return hosts;
                }
            }
            catch (DnsResponseException)
            {
                // Ignore DNS lookup failures and fall back to the domain itself.
            }

            return new[] { domain };
        }

        private static async Task<bool> TryConnectAsync(string host, CancellationToken cancellationToken)
        {
            try
            {
                using var client = new TcpClient();
                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                linkedCts.CancelAfter(ConnectionTimeout);

                await client.ConnectAsync(host, 25, linkedCts.Token).ConfigureAwait(false);

                return client.Connected;
            }
            catch (Exception ex) when (ex is SocketException or TaskCanceledException or OperationCanceledException)
            {
                return false;
            }
        }
    }
}
