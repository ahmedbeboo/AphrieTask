using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MailServices
{
    public interface IEmailSender
    {
        bool SendEmail(string Subject, string Body, string From, string FromName, string To, string ToName,
                       string ReplyTo, string ReplyToName, IEnumerable<string> bcc, IEnumerable<string> cc, string AttachmentFilePath,
                       string AttachmentFileName, int AttachedDownloadId, IDictionary<string, string> headers
                       );
    }
}
