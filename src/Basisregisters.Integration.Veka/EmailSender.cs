namespace Basisregisters.Integration.Veka
{
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Options;

    public interface IEmailSender
    {
        Task SendEmailFor(Melding melding);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailOptions _emailOptions;

        public EmailSender(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public async Task SendEmailFor(Melding melding)
        {
        }
    }
}
