using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Mail
{
    public class NullMailDeliveryService : IMailDeliveryService
    {
        public void DeliverPendingMessages()
        {
            
        }

        public IEnumerable<SentMailMessage> FindMessagesWhere(System.Linq.Expressions.Expression<Func<SentMailMessage, bool>> predicate)
        {
            return new List<SentMailMessage>();
        }

        public IEnumerable<MailDeliveryInfo> DeliverMessagesWhere(System.Linq.Expressions.Expression<Func<SentMailMessage, bool>> predicate)
        {
            return new List<MailDeliveryInfo>();
        }

        public IEnumerable<MailDeliveryInfo> SuspendMessagesWhere(System.Linq.Expressions.Expression<Func<SentMailMessage, bool>> predicate)
        {
            return new List<MailDeliveryInfo>();
        }

        public IEnumerable<MailDeliveryInfo> CancelMessagesWhere(System.Linq.Expressions.Expression<Func<SentMailMessage, bool>> predicate)
        {
            return new List<MailDeliveryInfo>();
        }

        public IEnumerable<MailDeliveryInfo> DeliverMessages(IEnumerable<SentMailMessage> messages)
        {
            return new List<MailDeliveryInfo>();
        }

        public IEnumerable<MailDeliveryInfo> SuspendMessages(IEnumerable<SentMailMessage> messages)
        {
            return new List<MailDeliveryInfo>();
        }

        public IEnumerable<MailDeliveryInfo> CancelMessages(IEnumerable<SentMailMessage> messages)
        {
            return new List<MailDeliveryInfo>();
        }

        public IEnumerable<MailAccount> GetMailAccounts()
        {
            return new List<MailAccount>();
        }

        public void AddMailAccount(MailAccount account)
        {

        }

        public void RemoveMailAccount(MailAccount account)
        {

        }
    }
}
