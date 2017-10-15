using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailBee.SmtpMail;
using MailBee.Security;
using MailBee.Mime;
using Discover.Logging;
using Discover.Emailing.Config;
using Discover.Emailing.MailBee.EmailDataTableAdapters;
using System.Web;

namespace Discover.Emailing.MailBee
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public class MailBeeEmailerWithSqlBackup : IEmailer
    {
        public MailBeeEmailerWithSqlBackup(ILogger logger)
        {
            _logger = logger;
        }

        private ILogger _logger;

        /// <summary>
        /// TODO: Not true DI - extract out at later date
        /// </summary>
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


        public void EmailException(Exception exception)
        {
            if (Enabled)
            {
                try
                {
                    StringBuilder message = new StringBuilder();
                    string title = EmailerConfig.ExceptionEmailTitle;
                    message.AppendFormat("<h1>{0}</h1>", title);
                    message.AppendFormat("<h3>{0}</h3><p>{1}</p>", "Date", DateTime.Now.ToString());
                    message.AppendFormat("<h3>{0}</h3><p>{1}</p>", "Message", exception.Message);
                    message.AppendFormat("<h3>{0}</h3><p>{1}</p>", "Stack trace", exception.ToString());

                    SendEmail(new EmailContact(EmailerConfig.AdministrativeAddress), title, message.ToString());

                }
                catch (Exception ex)
                {
                    _logger.Error(this.GetType(), ex, "Failed emailing exception.");
                }
            }
        }

        public List<Email> FindEmails(DateTime fromDate, DateTime toDate)
        {
            List<Email> emails = new List<Email>();
            EmailTableAdapter adapter = new EmailTableAdapter();
            EmailData.EmailDataTable table = adapter.FindEmails(fromDate, toDate);
            foreach(EmailData.EmailRow row in table){
                emails.Add(EmailerHelper.ToEmailDTO(row, GetEmailAccountByEmailAddress(row.FromEmailAddress), GetEmailAttachments(row.Id)));
            }
            return emails;
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
            SendEmail(to, GetEmailAccount(), subject, message, null, false, false, "");
        }

        public void SendEmail(EmailContact to, string subject, string message, List<Discover.Emailing.EmailAttachment> attachments, bool digitallySign, bool encrypt)
        {
            SendEmail(to, GetEmailAccount(), subject, message, attachments, digitallySign, encrypt, "");
        }

        public void SendEmail(EmailContact to, string subject, string message, List<Discover.Emailing.EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId)
        {
            SendEmail(to, GetEmailAccount(), subject, message, attachments, digitallySign, encrypt, batchId);
        }

        public void SendEmail(EmailContact to, EmailAccount from, string subject, string message)
        {
            SendEmail(to, from, subject, message, null, false, false, "");
        }

        public void SendEmail(EmailContact to, EmailAccount from, string subject, string message, List<Discover.Emailing.EmailAttachment> attachments, bool digitallySign, bool encrypt)
        {
            SendEmail(to, from, subject, message, attachments, digitallySign, encrypt, "");
        }

        public void SendEmail(EmailContact to, EmailAccount from, string subject, string message, List<Discover.Emailing.EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId)
        {
            List<EmailContact> tos = new List<EmailContact>();
            tos.Add(to);
            SendEmail(tos, from, subject, message, attachments, digitallySign, encrypt, batchId);
        }

        public void SendEmail(List<EmailContact> to, string subject, string message)
        {
            SendEmail(to, GetEmailAccount(), subject, message, null, false, false, "");
        }

        public void SendEmail(List<EmailContact> to, string subject, string message, List<Discover.Emailing.EmailAttachment> attachments)
        {
            SendEmail(to, GetEmailAccount(), subject, message, attachments, false, false, "");
        }

        public void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message)
        {
            SendEmail(to, from, subject, message, null, false, false, "");
        }

        public void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<Discover.Emailing.EmailAttachment> attachments)
        {
            SendEmail(to, from, subject, message, attachments, false, false, "");
        }

        public void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<Discover.Emailing.EmailAttachment> attachments, bool digitallySign, bool encrypt)
        {
            SendEmail(to, from, subject, message, attachments, digitallySign, encrypt, "");
        }

        public void SendEmail(List<EmailContact> to, EmailAccount from, string subject, string message, List<Discover.Emailing.EmailAttachment> attachments, bool digitallySign, bool encrypt, string batchId)
        {
            Email email = new Email();
            email.To.AddRange(to);
            email.From = from;
            email.Subject = subject;
            email.Message = message;
            if (attachments != null) email.Attachments.AddRange(attachments);
            email.BatchId = batchId;
            email.IsEncrypted = encrypt;
            email.IsSigned = digitallySign;

            if (encrypt)
            {
                SendEncrypted(email);
            }
            else if (digitallySign)
            {
                SendSigned(email);
            }
            else
            {
                Send(email);
            }
        }

        public void SendEmail(Email mail)
        {
            try
            {
                ValidateEmail(mail);
                Save(mail);
                if (EmailerConfig.Enabled)
                {
                    Send(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(this.GetType(), ex, "Error sending email");
            }
        }

        public void QueueEmail(Email mail)
        {
            ValidateEmail(mail);
            mail.SendingStatus = EmailSendStatus.PENDING;
            Save(mail);
        }

        public void QueueEmail(Email mail, EmailAccount from)
        {
            mail.From = from;
            QueueEmail(mail);
        }

        public void QueueAndSendEmail(Email mail)
        {
            QueueEmail(mail);
            SendQueuedEmails();
        }

        public void QueueAndSendEmail(Email mail, EmailAccount from)
        {
            QueueEmail(mail, from);
            SendQueuedEmails();
        }

        public List<Email> GetFailedEmails()
        {
            List<Email> emails = new List<Email>();
            EmailTableAdapter adapter = new EmailTableAdapter();
            EmailData.EmailDataTable table = adapter.GetAllFailedEmails();
            foreach (EmailData.EmailRow row in table)
            {
                emails.Add(EmailerHelper.ToEmailDTO(row, GetEmailAccountByEmailAddress(row.FromEmailAddress), GetEmailAttachments(row.Id)));
            }
            return emails;
        }

        public List<Email> GetSuspendedEmails()
        {
            List<Email> emails = new List<Email>();
            EmailTableAdapter adapter = new EmailTableAdapter();
            EmailData.EmailDataTable table = adapter.GetAllSuspendedEmails();
            foreach (EmailData.EmailRow row in table)
            {
                emails.Add(EmailerHelper.ToEmailDTO(row, GetEmailAccountByEmailAddress(row.FromEmailAddress), GetEmailAttachments(row.Id)));
            }
            return emails;
        }

        public List<Email> GetQueuedEmails()
        {
            List<Email> emails = new List<Email>();
            EmailTableAdapter adapter = new EmailTableAdapter();
            EmailData.EmailDataTable table = adapter.GetPendingEmails();
            foreach (EmailData.EmailRow row in table)
            {
                emails.Add(EmailerHelper.ToEmailDTO(row, GetEmailAccountByEmailAddress(row.FromEmailAddress), GetEmailAttachments(row.Id)));
            }
            return emails;
        }

        public List<Email> GetEmailsByBatch(string batchId)
        {
            List<Email> emails = new List<Email>();
            EmailTableAdapter adapter = new EmailTableAdapter();
            EmailData.EmailDataTable table = adapter.GetEmailsByBatchId(batchId);
            foreach (EmailData.EmailRow row in table)
            {
                emails.Add(EmailerHelper.ToEmailDTO(row, GetEmailAccountByEmailAddress(row.FromEmailAddress), GetEmailAttachments(row.Id)));
            }
            return emails;
        }

        public Email GetEmail(int emailId)
        {
            if (emailId <= 0) return null;
            EmailTableAdapter adapter = new EmailTableAdapter();
            EmailData.EmailDataTable table = adapter.GetEmail(emailId);
            if (table.Count == 0) return null;
            EmailData.EmailRow row = table[0];
            return EmailerHelper.ToEmailDTO(row, GetEmailAccountByEmailAddress(row.FromEmailAddress), GetEmailAttachments(row.Id));
        }

        public void SendQueuedEmails()
        {
            if (Enabled)
            {
                int max = EmailerConfig.ConnectionLimit;
                List<Email> emails = GetQueuedEmails();
                int count = emails.Count;
                if (count > 0)
                {
                    _logger.Log(this.GetType(), "Attempting to send queued emails");

                    if (count > max)
                    {
                        count = max;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        Email email = emails[i];
                        Send(email);

                    }
                }
            }

        }

        public void SendFailedEmails()
        {
            if (Enabled)
            {
                int max = EmailerConfig.ConnectionLimit;
                List<Email> emails = GetFailedEmails();
                int count = emails.Count;
                if (count > 0)
                {
                    _logger.Log(this.GetType(), "Attempting to send failed emails");

                    if (count > max)
                    {
                        count = max;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        Email email = emails[i];
                        Send(email);

                    }
                }
            }
        }

        public void SendAllEmails()
        {
            if (Enabled)
            {
                SendQueuedEmails();
                SendFailedEmails();
            }
        }

        public void UpdateEmailStatus(int id, EmailSendStatus status, string lastEditedBy)
        {
            if (id == 0) throw new ArgumentException(string.Format("Could not update email status - invalid email [{0}].", id));
             EmailTableAdapter adapter = new EmailTableAdapter();
            EmailData.EmailDataTable table = adapter.GetEmail(id);
            if (table.Count == 0) throw new ArgumentException(string.Format("Could not update email - email does not exist emailId=[{0}]", id));
            EmailData.EmailRow row = table[0];
            row.SendStatus = status.ToString();
            adapter.Update(row);
        }



        private List<EmailAttachment> GetEmailAttachments(int emailId)
        {
            List<EmailAttachment> attachments = new List<EmailAttachment>();
            EmailAttachmentTableAdapter adapter = new EmailAttachmentTableAdapter();
            EmailData.EmailAttachmentDataTable table = adapter.GetEmailAttachments(emailId);
            foreach (EmailData.EmailAttachmentRow row in table)
            {
                attachments.Add(new EmailAttachment(row.Id, row.Name, row.Path));
            }
            return attachments;
        }

        private void ValidateEmail(Email email)
        {
            if (email.From == null || string.IsNullOrEmpty(email.From.EmailAddress))
            {
                email.From = GetEmailAccount();
            }
        }

        private void Save(Email newEmail)
        {
            try
            {
                if (newEmail == null) throw new ArgumentException("Could not save email - email object is null!");
                EmailTableAdapter adapter = new EmailTableAdapter();
                if (string.IsNullOrEmpty(newEmail.CreatedBy)) newEmail.CreatedBy = "SYSTEM";
                string to = string.Join(";", (from t in newEmail.To select t.EmailAddress).ToArray());
                int? emailId = 0;
                adapter.Insert(newEmail.Guid,
                    newEmail.BatchId,
                    to,
                    newEmail.From.EmailAddress,
                    newEmail.From.DisplayName,
                    newEmail.Subject,
                    newEmail.Message,
                    newEmail.IsHtml,
                    DateTime.Now,
                    "",
                    0,
                    DateTime.Now,
                    newEmail.IsSigned,
                    newEmail.IsEncrypted,
                    newEmail.SendingStatus.ToString(),
                    newEmail.CreatedBy,
                    ref emailId);

                newEmail.Id = (int)emailId;

                foreach (EmailAttachment attach in newEmail.Attachments)
                {
                    SaveAttachment(newEmail.Id, attach);
                }
            }
            catch (Exception ex)
            {
                if (newEmail == null)
                {
                    _logger.Error(this.GetType(), ex, "Failed saving email.");
                }
                else
                {
                    _logger.Error(this.GetType(), ex, "Failed saving email id=[{0}] to=[{1}] subject=[{2}]", newEmail.Id, newEmail.To.ToString(), newEmail.Subject);
                }
            }

        }

        private void SaveAttachment(int emailId, EmailAttachment newAttachment)
        {
            EmailAttachmentTableAdapter adapter = new EmailAttachmentTableAdapter();
            int? id = 0;
            adapter.Insert(emailId, newAttachment.Name, newAttachment.Path, ref id);
            newAttachment.Id = (int)id;
        }

        private void UpdateEmail(Email email)
        {
            if (email == null || email.Id <= 0) throw new ArgumentException("Could not update email - email is null or has invalid id");
            EmailTableAdapter adapter = new EmailTableAdapter();
            EmailData.EmailDataTable table = adapter.GetEmail(email.Id);
            if (table.Count == 0) throw new ArgumentException(string.Format("Could not update email - email does not exist emailId=[{0}]", email.Id));
            EmailData.EmailRow row = table[0];
            row.FailedAttempts = email.FailAttempts;
            if (email.FailedDate != null) row.FailedDate = (DateTime)email.FailedDate;
            row.FailedException = email.FailedException;
            if (email.SendDate != null) row.SendDate = (DateTime)email.SendDate;
            row.SendStatus = email.SendingStatus.ToString();
            adapter.Update(row);
        }

        private bool Send(List<EmailContact> to, EmailAccount senderAccount, string subject, string message, bool isHtml, List<EmailAttachment> attachments)
        {
            Email email = new Email();
            email.To.AddRange(to);
            email.From = senderAccount;
            email.Subject = subject;
            email.Message = message;
            email.IsHtml = isHtml;
            email.Attachments.AddRange(attachments);
            Save(email);
            return Send(email);
        }

        private bool Send(Email email)
        {

            if (email.Id == 0) Save(email);
            if (Enabled)
            {
                Smtp oMailer = new Smtp();
                try
                {
                    if (email == null) throw new ArgumentException("Could not send email - email object is null!");

                    _logger.Info(this.GetType(), "Attempting to send email id=[{0}] to=[{1}] subject=[{2}] message=[{3}]", email.Id, email.To.ToString(), email.Subject, email.Message);

                    EmailAccount senderAccount = email.From;
                    if (senderAccount == null) throw new ArgumentException("Could not send email - sender account is null!");

                    oMailer.SmtpServers.Add(senderAccount.Server, senderAccount.ServerUsername, senderAccount.ServerPassword);
                    //Smime objSmime = new Smime();
                    MailMessage m = new MailMessage();
                    m.From = new EmailAddress(senderAccount.EmailAddress, senderAccount.DisplayName);

                    foreach (EmailContact c in email.To)
                    {
                        m.To.Add(c.EmailAddress, c.DisplayName);
                    }
                    m.Subject = email.Subject;
                    if (email.IsHtml)
                    {
                        m.BodyHtmlText = email.Message;
                    }
                    else
                    {
                        m.BodyPlainText = email.Message;
                    }
                    foreach (EmailAttachment a in email.Attachments)
                    {
                        m.Attachments.Add(a.Path, a.Name);
                    }

                    oMailer.Message = m;
                    oMailer.Message.ThrowExceptions = true;
                    //set utf 8 for international chars
                    oMailer.Message.Charset = "utf-8";
                    oMailer.Message.EncodeAllHeaders(System.Text.Encoding.UTF8, HeaderEncodingOptions.None);
                    if (!oMailer.Send()) throw new Exception("Failed sending email...");
                    string toStr = string.Join(",", (from t in email.To select t.EmailAddress).ToArray());
                    _logger.Info("Email successfully sent by MailBee.Net to=[{0}] subject=[{1}]", toStr, email.Subject);

                    email.SendingStatus = EmailSendStatus.SENT;
                    email.SendDate = DateTime.Now;
                    UpdateEmail(email);

                    return true;
                }
                catch (Exception ex)
                {
                    if (email == null)
                    {
                        _logger.Error(this.GetType(), ex, "Failed sending email.");
                        return false;
                    }

                    EmailAccount senderAccount = email.From;
                    string accountName = senderAccount == null || senderAccount.Name == null ? "" : senderAccount.Name;
                    string accountEmailAddress = senderAccount == null || senderAccount.EmailAddress == null ? "" : senderAccount.EmailAddress;
                    string accountServer = senderAccount == null || senderAccount.Server == null ? "" : senderAccount.Server;
                    int accountPort = senderAccount == null || senderAccount.Port == null ? 0 : senderAccount.Port;


                    string toStr = string.Join(",", (from t in email.To select t.EmailAddress).ToArray());
                    _logger.Error(this.GetType(), ex, "Failed sending email. emailId=[{6}] account=[{0}] from=[{1}] server=[{2}] port=[{3}] to=[{4}] subject=[{5}]", accountName, accountEmailAddress, accountServer, accountPort, toStr, email.Subject, email.Id);

                    email.FailAttempts += 1;
                    email.FailedDate = DateTime.Now;
                    email.FailedException = ex.ToString();
                    if (email.FailAttempts <= EmailerConfig.SuspendMailAfterAttempts)
                    {
                        email.SendingStatus = EmailSendStatus.SUSPENDED;
                    }
                    else
                    {
                        email.SendingStatus = EmailSendStatus.FAILED;
                    }
                    UpdateEmail(email);
                    return false;
                }
                finally
                {
                    oMailer.Dispose();
                    oMailer = null;
                }
            }
            return false;
        }

        private bool SendSigned(Email email)
        {
            if (email.Id == 0) Save(email);
            if (Enabled)
            {
                Smtp oMailer = new Smtp();
                try
                {
                    if (email == null) throw new ArgumentException("Could not send email - email object is null!");

                    _logger.Info(this.GetType(), "Attempting to send signed email id=[{0}] to=[{1}] subject=[{2}] message=[{3}]", email.Id, email.To.ToString(), email.Subject, email.Message);

                    EmailAccount senderAccount = email.From;
                    if (senderAccount == null) throw new ArgumentException("Could not send email - sender account is null!");

                    oMailer.SmtpServers.Add(senderAccount.Server, senderAccount.ServerUsername, senderAccount.ServerPassword);
                    Smime objSmime = new Smime();
                    MailMessage m = new MailMessage();
                    m.From = new EmailAddress(senderAccount.EmailAddress, senderAccount.DisplayName);

                    foreach (EmailContact c in email.To)
                    {
                        m.To.Add(c.EmailAddress, c.DisplayName);
                    }
                    m.Subject = email.Subject;
                    if (email.IsHtml)
                    {
                        m.BodyHtmlText = email.Message;
                    }
                    else
                    {
                        m.BodyPlainText = email.Message;
                    }
                    foreach (EmailAttachment a in email.Attachments)
                    {
                        m.Attachments.Add(a.Path, a.Name);
                    }

                    // Load certificate from the specified file.
                    Certificate signingCert = new Certificate(senderAccount.CertificateFilePath, CertFileType.Pfx, senderAccount.CertificatePassword);

                    // Sign the message.
                    MailMessage signMsg = objSmime.Sign(m, signingCert);

                    oMailer.Message = signMsg;

                    oMailer.Message.ThrowExceptions = true;
                    //set utf 8 for international chars
                    oMailer.Message.Charset = "utf-8";
                    oMailer.Message.EncodeAllHeaders(System.Text.Encoding.UTF8, HeaderEncodingOptions.None);
                    if (!oMailer.Send()) throw new Exception("Failed sending email...");
                    string toStr = string.Join(",", (from t in email.To select t.EmailAddress).ToArray());
                    _logger.Info("Email successfully sent by MailBee.Net to=[{0}] subject=[{1}]", toStr, email.Subject);

                    email.SendingStatus = EmailSendStatus.SENT;
                    email.SendDate = DateTime.Now;
                    UpdateEmail(email);

                    return true;
                }
                catch (Exception ex)
                {
                    if (email == null)
                    {
                        _logger.Error(this.GetType(), ex, "Failed sending signed email.");
                        return false;
                    }

                    EmailAccount senderAccount = email.From;
                    string accountName = senderAccount == null || senderAccount.Name == null ? "" : senderAccount.Name;
                    string accountEmailAddress = senderAccount == null || senderAccount.EmailAddress == null ? "" : senderAccount.EmailAddress;
                    string accountServer = senderAccount == null || senderAccount.Server == null ? "" : senderAccount.Server;
                    int accountPort = senderAccount == null || senderAccount.Port == null ? 0 : senderAccount.Port;


                    string toStr = string.Join(",", (from t in email.To select t.EmailAddress).ToArray());
                    _logger.Error(this.GetType(), ex, "Failed sending signed email. emailId=[{6}] account=[{0}] from=[{1}] server=[{2}] port=[{3}] to=[{4}] subject=[{5}]", accountName, accountEmailAddress, accountServer, accountPort, toStr, email.Subject, email.Id);

                    email.FailAttempts += 1;
                    email.FailedDate = DateTime.Now;
                    email.FailedException = ex.ToString();
                    if (email.FailAttempts <= EmailerConfig.SuspendMailAfterAttempts)
                    {
                        email.SendingStatus = EmailSendStatus.FAILED;
                    }
                    else
                    {
                        email.SendingStatus = EmailSendStatus.SUSPENDED;
                    }
                    UpdateEmail(email);

                    return false;
                }
                finally
                {
                    oMailer.Dispose();
                    oMailer = null;
                }
            }
            return false;
        }

        private bool SendEncrypted(Email email)
        {
            if (email.Id == 0) Save(email);
            if (Enabled)
            {
                Smtp oMailer = new Smtp();
                try
                {
                    if (email == null) throw new ArgumentException("Could not send email - email object is null!");

                    _logger.Info(this.GetType(), "Attempting to send encrypted email id=[{0}] to=[{1}] subject=[{2}] message=[{3}]", email.Id, email.To.ToString(), email.Subject, email.Message);

                    EmailAccount senderAccount = email.From;
                    if (senderAccount == null) throw new ArgumentException("Could not send email - sender account is null!");

                    oMailer.SmtpServers.Add(senderAccount.Server, senderAccount.ServerUsername, senderAccount.ServerPassword);
                    //Smime objSmime = new Smime();
                    MailMessage m = new MailMessage();
                    m.From = new EmailAddress(senderAccount.EmailAddress, senderAccount.DisplayName);

                    foreach (EmailContact c in email.To)
                    {
                        m.To.Add(c.EmailAddress, c.DisplayName);
                    }
                    m.Subject = email.Subject;
                    if (email.IsHtml)
                    {
                        m.BodyHtmlText = email.Message;
                    }
                    else
                    {
                        m.BodyPlainText = email.Message;
                    }
                    foreach (EmailAttachment a in email.Attachments)
                    {
                        m.Attachments.Add(a.Path, a.Name);
                    }

                    oMailer.Message = m;
                    oMailer.Message.ThrowExceptions = true;
                    //set utf 8 for international chars
                    oMailer.Message.Charset = "utf-8";
                    oMailer.Message.EncodeAllHeaders(System.Text.Encoding.UTF8, HeaderEncodingOptions.None);
                    if (!oMailer.Send()) throw new Exception("Failed sending email...");
                    string toStr = string.Join(",", (from t in email.To select t.EmailAddress).ToArray());
                    _logger.Info("Email successfully sent by MailBee.Net to=[{0}] subject=[{1}]", toStr, email.Subject);

                    email.SendingStatus = EmailSendStatus.SENT;
                    email.SendDate = DateTime.Now;
                    UpdateEmail(email);

                    return true;
                }
                catch (Exception ex)
                {
                    if (email == null)
                    {
                        _logger.Error(this.GetType(), ex, "Failed sending mail.");
                        return false;
                    }

                    EmailAccount senderAccount = email.From;
                    string accountName = senderAccount == null || senderAccount.Name == null ? "" : senderAccount.Name;
                    string accountEmailAddress = senderAccount == null || senderAccount.EmailAddress == null ? "" : senderAccount.EmailAddress;
                    string accountServer = senderAccount == null || senderAccount.Server == null ? "" : senderAccount.Server;
                    int accountPort = senderAccount == null || senderAccount.Port == null ? 0 : senderAccount.Port;

                    string toStr = string.Join(",", (from t in email.To select t.EmailAddress).ToArray());
                    _logger.Error(this.GetType(), ex, "Failed sending mail. emailId=[{6}] account=[{0}] from=[{1}] server=[{2}] port=[{3}] to=[{4}] subject=[{5}]", accountName, accountEmailAddress, accountServer, accountPort, toStr, email.Subject, email.Id);

                    email.FailAttempts += 1;
                    email.FailedDate = DateTime.Now;
                    email.FailedException = ex.ToString();
                    if (email.FailAttempts <= EmailerConfig.SuspendMailAfterAttempts)
                    {
                        email.SendingStatus = EmailSendStatus.FAILED;
                    }
                    else
                    {
                        email.SendingStatus = EmailSendStatus.SUSPENDED;
                    }
                    UpdateEmail(email);

                    return false;
                }
                finally
                {
                    oMailer.Dispose();
                    oMailer = null;
                }
            }
            return false;
        }



        public void Enable()
        {
            throw new NotImplementedException();
        }

        public void Disable()
        {
            throw new NotImplementedException();
        }

        public void InitDatabaseSchema()
        {
            var commands = new string[0];

            using (var script = new System.IO.StreamReader(this.GetType().Assembly.GetManifestResourceStream(this.GetType().Assembly.GetName().Name + ".MailBeeEmailerWithSqlBackup.sql")))
            {
                commands = script.ReadToEnd().Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
            }

            using(var adapter = new EmailTableAdapter())
            {
                adapter.Connection.Open();

                var cmd = adapter.Connection.CreateCommand();

                foreach (var command in commands)
                {
                    cmd.CommandText = command;
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
