using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.Emailing.Config;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class DummyEmailer : IEmailer
    {
        public Config.EmailerConfigSection EmailerConfig
        {
            get
            {
                return Config.EmailerConfigSection.Current;
            }
        }

        public bool Enabled
        {
            get
            {
                return EmailerConfig.Enabled;
            }
        }

        public void Enable()
        {

        }

        public void Disable()
        {

        }

        public void EmailException(Exception exception)
        {

        }

        public List<EmailAccount> GetAllAccounts()
        {
            List<EmailAccount> accounts = new List<EmailAccount>();
            foreach (Account account in EmailerConfig.Accounts)
            {
                accounts.Add(EmailAccountHelper.ToEmailAccountDTO(account));
            }
            return accounts;
        }

        public EmailAccount GetEmailAccount()
        {
            List<EmailAccount> accounts = GetAllAccounts();
            string accountName = EmailerConfig.DefaultAccountName.ToLower();
            if (!string.IsNullOrEmpty(accountName) && accounts.Count > 0)
            {
                EmailAccount acc = (from a in accounts where a.Name.ToLower() == accountName select a).SingleOrDefault();
                return acc;
            }
            return null;
        }

        public EmailAccount GetEmailAccount(string accountName)
        {
            List<EmailAccount> accounts = GetAllAccounts();
            accountName = accountName.ToLower();
            if (!string.IsNullOrEmpty(accountName) && accounts.Count > 0)
            {
                EmailAccount acc = (from a in accounts where a.Name.ToLower() == accountName select a).SingleOrDefault();
                return acc;
            }
            return null;
        }

        public EmailAccount GetEmailAccountByEmailAddress(string emailAddress)
        {
            List<EmailAccount> accounts = GetAllAccounts();
            emailAddress = emailAddress.ToLower();
            if (!string.IsNullOrEmpty(emailAddress) && accounts.Count > 0)
            {
                EmailAccount acc = (from a in accounts where a.EmailAddress.ToLower() == emailAddress select a).SingleOrDefault();
                return acc;
            }
            return null;
        }

        public void AddEmailAccount(string accountName, string fromName, string fromEmailAddress, string server, string port, string serverUsername, string serverPassword, string certFilePath, string certPassword)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(EmailContact to, string subject, string message)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(EmailContact to, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(EmailContact to, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(EmailContact to, EmailAccount from, string subject, string message)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(EmailContact to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(EmailContact to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(List<EmailContact> to, string subject, string message)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(List<EmailContact> to, string subject, string message, List<EmailAttachment> attachments)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId)
        {
            throw new NotImplementedException();
        }

        public void SendEmail(Email mail)
        {
            throw new NotImplementedException();
        }

        public void QueueEmail(Email mail)
        {
            throw new NotImplementedException();
        }

        public void QueueEmail(Email mail, EmailAccount from)
        {
            throw new NotImplementedException();
        }

        public void QueueAndSendEmail(Email mail)
        {
            throw new NotImplementedException();
        }

        public void QueueAndSendEmail(Email mail, EmailAccount from)
        {
            throw new NotImplementedException();
        }

        public List<Email> GetFailedEmails()
        {
            throw new NotImplementedException();
        }

        public List<Email> GetQueuedEmails()
        {
            throw new NotImplementedException();
        }

        public List<Email> GetEmailsByBatch(string batchId)
        {
            throw new NotImplementedException();
        }

        public Email GetEmail(int emailId)
        {
            throw new NotImplementedException();
        }

        public void SendQueuedEmails()
        {
            throw new NotImplementedException();
        }

        public void SendFailedEmails()
        {
            throw new NotImplementedException();
        }

        public void SendAllEmails()
        {
            throw new NotImplementedException();
        }


        public List<Email> FindEmails(DateTime fromDate, DateTime toDate)
        {
            throw new NotImplementedException();
        }


        public List<Email> GetSuspendedEmails()
        {
            throw new NotImplementedException();
        }

        public void UpdateEmailStatus(int id, EmailSendStatus status, string lastEditedBy)
        {
            throw new NotImplementedException();
        }
    }
}
