namespace Basisregisters.Integration.Veka
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Amazon.SimpleEmail;
    using Amazon.SimpleEmail.Model;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public interface IEmailSender
    {
        Task SendEmailFor(Melding melding);
    }

    public class EmailSender : IEmailSender
    {
        private readonly IAmazonSimpleEmailService _amazonSimpleEmailService;
        private readonly VekaOptions _vekaOptions;
        private readonly EmailOptions _emailOptions;
        private readonly ILogger _logger;

        public EmailSender(
            IAmazonSimpleEmailService amazonSimpleEmailService,
            IOptions<EmailOptions> emailOptions,
            IOptions<VekaOptions> vekaOptions,
            ILoggerFactory loggerFactory)
        {
            _amazonSimpleEmailService = amazonSimpleEmailService;
            _vekaOptions = vekaOptions.Value;
            _emailOptions = emailOptions.Value;
            _logger = loggerFactory.CreateLogger(GetType());
        }

        public async Task SendEmailFor(Melding melding)
        {
            if (!_emailOptions.Enabled)
            {
                _logger.LogInformation("Email sending is disabled, not sending email for melding {MeldingId}.", melding.Id);
                return;
            }

            await _amazonSimpleEmailService.SendEmailAsync(
                new SendEmailRequest
                {
                    Destination = new Destination
                    {
                        ToAddresses = new List<string> { _vekaOptions.EmailAddress }
                    },
                    Message = new Message
                    {
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = GetBodyHtml(melding)
                            }
                        },
                        Subject = new Content
                        {
                            Charset = "UTF-8",
                            Data = GetSubject(melding)
                        }
                    },
                    Source = _emailOptions.SenderEmail
                });
        }

        private string GetBodyHtml(Melding melding)
        {
            var body =
                $@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd""><html xmlns=""http://www.w3.org/1999/xhtml"">
                <head>
                    <title>Melding afgesloten</title>
                    {GetStyle()}
                </head>
                <body>
                <p>Beste,</p>

                    <p>Er werd een melding afgerond.</p>

                    <p><b>Overzicht van de melding:</b></p>

                    <table class=""reference"" cellpadding=""0"" cellspacing=""0"" border=""1px"">
                        <tr>
                            <td class=""overzichtcol1"">Indiendatum:</td>
                            <td class=""overzichtcol2"">{melding.DatumVaststelling:dd-MM-yyyy}</td>
                        </tr>
                        <tr>
                            <td class=""overzichtcol1"">Melding Id:</td>
                            <td class=""overzichtcol2"">{melding.Id}</td>
                        </tr>

                        {GetReferentie(melding.ReferentieMelder)}

                        <tr>
                            <td class=""overzichtcol1"">Beschrijving:</td>
                            <td class=""overzichtcol2"">{melding.Beschrijving}</td>
                        </tr>

                        <tr>
                            <td class=""overzichtcol1"">Status:</td>
                            <td class=""overzichtcol2"">{melding.Status}</td>
                        </tr>

                        <tr>
                            <td class=""overzichtcol1"">Toelichting:</td>
                            <td class=""overzichtcol2"">{melding.ToelichtingBehandelaar}</td>
                        </tr>
                    </table>

                    <p>
                        Uw melding werd behandeld door:<br>
                        {melding.Behandelaar}
                    </p>
                </body>
                </html>";
            return body;
        }

        private string GetReferentie(string? referentieMelder)
        {
            if (string.IsNullOrWhiteSpace(referentieMelder))
            {
                return string.Empty;
            }

            return @$" <tr>
                            <td class=""overzichtcol1"">Uw referentie:</td>
                            <td class=""overzichtcol2"">{referentieMelder}</td>
                        </tr>";
        }

        private string GetSubject(Melding melding)
        {
            if (string.IsNullOrWhiteSpace(melding.ReferentieMelder))
            {
                return $"Melding afgesloten met referentie: {melding.Id}";
            }

            return @$"Melding afgesloten met uw referentie: {melding.ReferentieMelder}";
        }

        private string GetStyle()
        {
            return @"<style type=""text/css"">

                        table.reference
                        {
                            background-color:#ffffff;
                            border:1px solid #000000;
                            border-collapse:collapse;
                        }

                        td.overzichtcol1
                        {
                            border: 1px solid #000000;
                            width:200px;
                            padding-left: 2px;
                        }

                        td.overzichtcol2
                        {
                            border: 1px solid #000000;
                            width:600px;
                            padding-left: 2px;
                        }

                        td.colbijlagen
                        {
                            border: 1px solid #000000;
                            width:300px;
                            padding-left: 2px;
                        }

                        td.colsituatie
                        {
                            border: 1px solid #000000;
                            width:800px;
                            padding-left: 2px;
                        }

                    </style>";
        }
    }
}
