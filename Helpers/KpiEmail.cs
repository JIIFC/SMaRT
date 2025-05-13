using MailKit.Net.Smtp;

using MimeKit;

namespace SMARTV3.Helpers
{
    public class KpiEmail
    {
        private readonly IConfiguration _configuration;
        private readonly string smtpAddress;
        private readonly int smtpPort;
        private readonly bool isDevelopment;

        public KpiEmail(IConfiguration configuration, bool isDevelopment)
        {
            _configuration = configuration;
            smtpAddress = _configuration.GetValue<string>("SMTP_Address");
            smtpPort = int.Parse(_configuration.GetValue<string>("Smtp_Port"));
            this.isDevelopment = isDevelopment;
        }

        public void SendEmail(string receiverEmail, string emailBody)
        {
            if (!isDevelopment)
            {
                try
                {
                    MimeMessage email = new();
                    email.From.Add(new MailboxAddress("SMaRT Alerts", "smart-ogdos@forces.cmil.ca"));
                    email.To.Add(new MailboxAddress("Receiver Name", receiverEmail));
                    email.Subject = "[SECRET AUS/CAN/NZ/UK/US EYES ONLY] SMaRT Automated Alert";
                    string emailClass = "<b>CLASSIFICATION: </b><b style=\"color: red;\">SECRET AUS/CAN/NZ/UK/US EYES ONLY</b>";
                    email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = "<div style=\"font-family: Calibri, sans-serif\">"
                        + emailClass + "<br><br>" + emailBody + "<br><br>" + emailClass
                        + "</div>"
                    };
                    using SmtpClient smtp = new();
                    smtp.Connect(smtpAddress, smtpPort, false);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                Console.WriteLine(emailBody);
            }
        }
    }
}