using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Discover.Mail
{
    /// <summary>
    /// Provides back-end delivery and management of emails
    /// </summary>
    public interface IMailDeliveryService
    {
        void DeliverPendingMessages();

        IEnumerable<SentMailMessage> FindMessagesWhere(Expression<Func<SentMailMessage, bool>> predicate);

        IEnumerable<MailDeliveryInfo> DeliverMessagesWhere(Expression<Func<SentMailMessage, bool>> predicate);

        IEnumerable<MailDeliveryInfo> SuspendMessagesWhere(Expression<Func<SentMailMessage, bool>> predicate);

        IEnumerable<MailDeliveryInfo> CancelMessagesWhere(Expression<Func<SentMailMessage, bool>> predicate);
        
        IEnumerable<MailDeliveryInfo> DeliverMessages(IEnumerable<SentMailMessage> messages);

        IEnumerable<MailDeliveryInfo> SuspendMessages(IEnumerable<SentMailMessage> messages);

        IEnumerable<MailDeliveryInfo> CancelMessages(IEnumerable<SentMailMessage> messages);

        IEnumerable<MailAccount> GetMailAccounts();

        void AddMailAccount(MailAccount account);

        void RemoveMailAccount(MailAccount account);
    }
}
