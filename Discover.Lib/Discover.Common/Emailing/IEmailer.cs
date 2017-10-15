using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public interface IEmailer
    {
        void Enable();

        void Disable();

        void EmailException(Exception exception);

        List<Email> FindEmails(DateTime fromDate, DateTime toDate);

        List<EmailAccount> GetAllAccounts();

        EmailAccount GetEmailAccount();

        EmailAccount GetEmailAccount(string accountName);

        EmailAccount GetEmailAccountByEmailAddress(string emailAddress);

        void AddEmailAccount(string accountName, string fromName, string fromEmailAddress, string server, string port, string serverUsername, string serverPassword, string certFilePath, string certPassword);

        void SendEmail(EmailContact to, string subject, string message);

        void SendEmail(EmailContact to, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt);

        void SendEmail(EmailContact to, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId);

        void SendEmail(EmailContact to, EmailAccount from, string subject, string message);

        void SendEmail(EmailContact to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt);

        void SendEmail(EmailContact to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId);

        void SendEmail(List<EmailContact> to, string subject, string message);

        void SendEmail(List<EmailContact> to, string subject, string message, List<EmailAttachment> attachments);

        void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message);

        void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments);

        void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt);

        void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId);

        void SendEmail(Email mail);

        void QueueEmail(Email mail);

        void QueueEmail(Email mail, EmailAccount from);

        void QueueAndSendEmail(Email mail);

        void QueueAndSendEmail(Email mail, EmailAccount from);

        List<Email> GetSuspendedEmails();

        List<Email> GetFailedEmails();

        List<Email> GetQueuedEmails();

         List<Email> GetEmailsByBatch(string batchId);

        Email GetEmail(int emailId);

        void SendQueuedEmails();

        void SendFailedEmails();

        void SendAllEmails();

        void UpdateEmailStatus(int id, EmailSendStatus status, string lastEditedBy);

    }
}
