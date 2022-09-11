using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TBlog.Core.Models;

namespace TBlog.Core.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public Task SendEmailAsync(string email, string subject, string message, string link)
        {
            try
            {
                // Credentials
                var credentials = new NetworkCredential(_emailSettings.Sender, _emailSettings.Password);

                // Mail message
                var mail = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.Sender, _emailSettings.SenderName),
                    Subject = subject,
                    //Body = message,
                    Body = GetEmailTemplate(subject, message, link),
                    IsBodyHtml = true
                };

                mail.To.Add(new MailAddress(email));

                // Smtp client
                var client = new SmtpClient()
                {
                    Port = _emailSettings.MailPort,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Host = _emailSettings.MailServer,
                    EnableSsl = false,  // Might enable this
                    Credentials = credentials
                };

                // Send it...         
                client.Send(mail);
            }
            catch (Exception ex)
            {
                // TODO: handle exception
                throw new InvalidOperationException(ex.Message);
            }

            return Task.CompletedTask;
        }


        private string GetEmailTemplate(string subject, string message, string link)
        {
            StringBuilder sb = new StringBuilder(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
    <title>Demystifying Email Design</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
</head>
<body style=""margin: 0; padding: 0;"">
    <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
        <tr>
            <td style=""padding: 10px 0 30px 0;"">
                <table align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""border: 1px solid #cccccc; border-collapse: collapse;"">
                    <tr>
                        <td bgcolor=""#212529"" style=""padding: 15px 15px 15px 15px; color: #153643; font-size: 28px; font-weight: bold; font-family: Arial, sans-serif;"">
                            <table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""600"" border-collapse: collapse;>
                                <tr>
                                    <td align=""left"" width=""50%"">
                                        <img src=""http://tblog.co.uk/img/tblog-logo-30.png"" alt=""Creating Email Magic"" width=""26"" height=""30"" style=""display: block;"" />
                                    </td>
                                    <td width=""50%"" align=""right"" style=""font-size:16px; color:#FFFFFF; font-family: Arial, sans-serif;"">
                                        <a href=""http://tblog.co.uk"" style=""color:#FFFFFF; text-decoration:none; font-family: Arial, sans-serif;"">T-BLOG</a>
                                        &nbsp; &nbsp;|&nbsp; &nbsp; 
                                        <a href=""http://tblog.co.uk"" style=""color:#FFFFFF; text-decoration:none; font-family: Arial, sans-serif;"">CONTACT</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor=""#ffffff"" style=""padding: 40px 30px 40px 30px;"">
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td style=""color: #153643; font-family: Arial, sans-serif; font-size: 24px;"">
                                        <b>#Subject#</b>
                                    </td>
                                </tr>
                                <tr>
                                    <td style=""padding: 20px 0 30px 0; color: #153643; font-family: Arial, sans-serif; font-size: 16px; line-height: 20px;"">
                                        #Message#
                                    </td>
                                </tr>
                                <tr>
                                    <td align=""center"">
                                        #Link#
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td bgcolor=""AAAAAA"" style=""padding: 15px 15px 15px 15px;"">
                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                <tr>
                                    <td style=""color: #ffffff; font-family: Arial, sans-serif; font-size: 14px;"" width=""75%"">
                                        &reg; T-Blog #Year#<br />
                                        <a href=""http://www.tblog.co.uk/"" style=""color: #ffffff;""><font color=""#ffffff"">Unsubscribe</font></a> from T-Blog
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>");

            sb.Replace("#Subject#", subject);
            sb.Replace("#Message#", message);
            sb.Replace("#Link#", link);
            sb.Replace("#Year#", DateTime.Now.Year.ToString());

            return sb.ToString();
        }

    }
}