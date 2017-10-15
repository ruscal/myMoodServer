using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Emailing.Config;

namespace Discover.Emailing.MailBee
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    class EmailerHelper
    {


        public static Email ToEmailDTO(EmailData.EmailRow row, EmailAccount from, List<EmailAttachment> attachments)
        {
            string[] tos = row.To.Split(';');
            List<EmailContact> toList = (from t in tos select new EmailContact(t)).ToList();
            Email email = new Email(row.Id,
                row.Guid,
                from,
                toList,
                row.Subject,
                row.Message,
                row.IsHtml,
                row.BatchId,
                row.SendDate,
                row.IsSigned,
                row.IsEncrypted,
                (EmailSendStatus)Enum.Parse(typeof(EmailSendStatus), row.SendStatus),
                row.DateCreated,
                row.CreatedBy,
                row.LastEdited,
                row.LastEditedBy);

            if(attachments != null) email.Attachments.AddRange(attachments);
            return email;

        }
    }
}
