using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discover.DomainModel;
using MailBee.SmtpMail;
using MailBee.Security;
using MailBee.Mime;
using MailBeeMessage = MailBee.Mime.MailMessage;
using MailBeeEmailAddress = MailBee.Mime.EmailAddress;

namespace Discover.Mail.MailBee
{
    public class MailBeeMailServiceProvider : IMailDispatchService, IMailDeliveryService
    {
        private readonly IDomainDataContext db;
        private readonly Discover.Mail.Config.MailConfigSection config;

        public MailBeeMailServiceProvider(IDomainDataContext db)
        {
            this.db = db;
            this.config = Discover.Mail.Config.MailConfigSection.Current;
        }

        #region IMailDispatchService Implementation 

        public Guid Send(MailMessage message)
        {
            return Send(message, new MailOptions());
        }

        public Guid Send(MailMessage message, MailOptions options)
        {
            var messageForDispatch = new SentMailMessage(message)
            {
                IsSigned = options.Sign,
                IsEncrypted = options.Encrypt,
                DeliveryStatus = options.Suspend ? DeliveryStatus.Suspended : DeliveryStatus.Pending,
                DelayUntil = options.DelayUntil
            };


            db.Add(messageForDispatch);

            return messageForDispatch.Id;
        }

        public MailDeliveryInfo GetDeliveryInfoFor(Guid messageId)
        {
            return (from m in db.Get<SentMailMessage>()
                    where m.Id == messageId
                    select new MailDeliveryInfo
                    {
                        MessageId = m.Id,
                        DeliveryStatus = m.DeliveryStatus,
                        SentOn = m.SentOn,
                        FailedOn = m.FailedOn,
                        FailureCount = m.FailureCount,
                        FailureMessage = m.FailureMessage
                    })
                    .SingleOrDefault();
        }

        public IEnumerable<MailDeliveryInfo> GetDeliveryInfoFor(IEnumerable<Guid> messageIds)
        {
            return (from m in db.Get<SentMailMessage>()
                    where messageIds.Contains(m.Id)
                    select new MailDeliveryInfo
                    {
                        MessageId = m.Id,
                        DeliveryStatus = m.DeliveryStatus,
                        SentOn = m.SentOn,
                        FailedOn = m.FailedOn,
                        FailureCount = m.FailureCount,
                        FailureMessage = m.FailureMessage
                    });
        }

        #endregion

        #region IMailDeliveryService Implementation

        public void DeliverPendingMessages()
        {
            if (config.Enabled)
            {
                var elapsed = DateTime.UtcNow.AddMinutes(-config.FailedMailRetryPeriod);
                var now = DateTime.UtcNow;
                var messages = (from m in db.Get<SentMailMessage>()
                                where (m.DelayUntil == null || m.DelayUntil <= now) && 
                                      (m.DeliveryStatus == DeliveryStatus.Pending || (m.DeliveryStatus == DeliveryStatus.Failed && m.FailedOn < elapsed))
                                select m);

                DeliverMessages(messages);
            }
        }

        public IEnumerable<SentMailMessage> FindMessagesWhere(System.Linq.Expressions.Expression<Func<SentMailMessage, bool>> predicate)
        {
            return db.Get<SentMailMessage>().Where(predicate);
        }

        public IEnumerable<MailDeliveryInfo> DeliverMessagesWhere(System.Linq.Expressions.Expression<Func<SentMailMessage, bool>> predicate)
        {
            return DeliverMessages(FindMessagesWhere(predicate));
        }

        public IEnumerable<MailDeliveryInfo> SuspendMessagesWhere(System.Linq.Expressions.Expression<Func<SentMailMessage, bool>> predicate)
        {
            return SuspendMessages(FindMessagesWhere(predicate));
        }

        public IEnumerable<MailDeliveryInfo> CancelMessagesWhere(System.Linq.Expressions.Expression<Func<SentMailMessage, bool>> predicate)
        {
            return CancelMessages(FindMessagesWhere(predicate));
        }

        public IEnumerable<MailDeliveryInfo> DeliverMessages(IEnumerable<SentMailMessage> messages)
        {
            var results = new List<MailDeliveryInfo>();

            foreach (var message in messages.ToArray())
            {
                TryDeliverMessage(message);

                results.Add(message.GetMailDeliveryInfo());
            }

            return results;
        }

        public IEnumerable<MailDeliveryInfo> SuspendMessages(IEnumerable<SentMailMessage> messages)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MailDeliveryInfo> CancelMessages(IEnumerable<SentMailMessage> messages)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MailAccount> GetMailAccounts()
        {
            return config.Accounts.Cast<Config.MailAccount>().Select(a => new MailAccount(a)).ToArray();
        }

        public void AddMailAccount(MailAccount account)
        {
            throw new NotImplementedException();
        }

        public void RemoveMailAccount(MailAccount account)
        {
            throw new NotImplementedException();
        }

        #endregion

        private MailAccount FindMailAccountFor(string senderAddress)
        {
            return config.Accounts.Cast<Config.MailAccount>().Where(a => a.SenderEmailAddress.Equals(senderAddress, StringComparison.InvariantCultureIgnoreCase)).Select(a => new MailAccount(a)).FirstOrDefault() ??
                config.Accounts.Cast<Config.MailAccount>().Where(a => a.Name.Equals(config.DefaultAccountName, StringComparison.InvariantCultureIgnoreCase)).Select(a => new MailAccount(a)).FirstOrDefault();
        }

        private bool TryDeliverMessage(SentMailMessage message)
        {
            using (var mailClient = new Smtp())
            {
                try
                {
                    var senderAccount = FindMailAccountFor(message.From.Address);

                    if (senderAccount == null) throw new ArgumentException("Could not deliver message - invalid account");

                    if (message.ReplyTo.Count() == 0 && !string.IsNullOrWhiteSpace(senderAccount.ReplyTo)) message.ReplyTo.Add(new MailAddress() { Address = senderAccount.ReplyTo });

                    var server = mailClient.SmtpServers.Add(senderAccount.ServerAddress, senderAccount.ServerUsername, senderAccount.ServerPassword);
                    server.Port = senderAccount.ServerPort;
                    server.Timeout = 60000;

                    var email = new MailBeeMessage() { Charset = "utf-8", ThrowExceptions = true };

                    email.From = new MailBeeEmailAddress(senderAccount.SenderEmailAddress, message.From.DisplayName ?? senderAccount.SenderDisplayName);
                    message.To.ForEach(to => email.To.Add(to.Address, to.DisplayName));
                    message.ReplyTo.ForEach(to => email.ReplyTo.Add(to.Address, to.DisplayName));
                    message.CC.ForEach(cc => email.Cc.Add(cc.Address, cc.DisplayName));
                    message.BCC.ForEach(bcc => email.Bcc.Add(bcc.Address, bcc.DisplayName));

                    email.Subject = message.Subject;
                    email.BodyPlainText = message.IsBodyHtml ? null : message.Body;
                    email.BodyHtmlText = message.IsBodyHtml ? message.Body : null;

                    foreach (var attachment in message.Attachments)
                    {
                        if (attachment.Content != null)
                        {
                            email.Attachments.Add(attachment.Content, attachment.Name, string.Empty, attachment.ContentType, null, NewAttachmentOptions.None, MailTransferEncoding.None);
                        }
                        else if (!string.IsNullOrEmpty(attachment.Path))
                        {
                            email.Attachments.Add(attachment.Path, attachment.Name, string.Empty, attachment.ContentType, null, NewAttachmentOptions.None, MailTransferEncoding.None);
                        }
                    }

                    if (message.IsEncrypted)
                    {
                        email = new Smime().SignAndEncrypt(email, new Certificate(senderAccount.CertificateFilePath, CertFileType.Pfx, senderAccount.CertificatePassword), FindAllRecipientCertificatesFor(message));
                    }
                    else if (message.IsSigned)
                    {
                        email = new Smime().Sign(email, new Certificate(senderAccount.CertificateFilePath, CertFileType.Pfx, senderAccount.CertificatePassword));
                    }

                    email.EncodeAllHeaders(System.Text.Encoding.UTF8, HeaderEncodingOptions.None);

                    mailClient.Message = email;
                    mailClient.Send();

                    message.DeliveryStatus = DeliveryStatus.Sent;
                    message.SentOn = DateTime.UtcNow;
                    return true;
                }
                catch (Exception ex)
                {
                    message.FailedOn = DateTime.UtcNow;
                    message.FailureCount += 1;
                    message.FailureMessage = ex.ToString();
                    message.DeliveryStatus = (message.FailureCount < config.SuspendMailAfterAttempts) ? DeliveryStatus.Failed : DeliveryStatus.Suspended;
                    return false;
                }
                finally
                {
                    db.SaveChanges();
                }
            }
        }

        private CertificateCollection FindAllRecipientCertificatesFor(MailMessage message)
        {
            var results = new CertificateCollection();

            var certStore = new CertificateStore(CertificateStore.OtherPeople, CertStoreType.System, null);
            
            foreach (var recipient in message.To.Concat(message.CC).Concat(message.BCC))
            {
                foreach (Certificate cert in certStore.FindCertificates(recipient.Address, CertificateFields.EmailAddress))
                {
                    results.Add(cert);
                }
            }

            return results;
        }
    }
}
