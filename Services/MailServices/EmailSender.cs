using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Services.MailServices
{
    public class EmailSender : IEmailSender
    {
        public EmailSender()
        {
        }

        public bool SendEmail(string subject, string body,
           string fromAddress, string fromName, string toAddress, string toName,
            string replyTo = null, string replyToName = null,
           IEnumerable<string> bcc = null, IEnumerable<string> cc = null,
           string attachmentFilePath = null, string attachmentFileName = null,
           int attachedDownloadId = 0, IDictionary<string, string> headers = null)
        {
            try
            {
                var message = new MailMessage
                {
                    //from, to, reply to
                    From = new MailAddress(fromAddress, fromName)
                };
                message.To.Add(new MailAddress(toAddress, toName));
                if (!string.IsNullOrEmpty(replyTo))
                {
                    message.ReplyToList.Add(new MailAddress(replyTo, replyToName));
                }

                //BCC
                if (bcc != null)
                {
                    foreach (var address in bcc.Where(bccValue => !string.IsNullOrWhiteSpace(bccValue)))
                    {
                        message.Bcc.Add(address.Trim());
                    }
                }

                //CC
                if (cc != null)
                {
                    foreach (var address in cc.Where(ccValue => !string.IsNullOrWhiteSpace(ccValue)))
                    {
                        message.CC.Add(address.Trim());
                    }
                }

                //content
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                //headers
                if (headers != null)
                    foreach (var header in headers)
                    {
                        message.Headers.Add(header.Key, header.Value);
                    }


                //send email
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Host = "smtp.gmail.com";
                    smtpClient.Port = 587;
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new System.Net.NetworkCredential("tt436209@gmail.com", "password123789");

                    smtpClient.Send(message);
                }

                return true;
            }
            catch
            {
                return false;
            }

        }
    }

}
