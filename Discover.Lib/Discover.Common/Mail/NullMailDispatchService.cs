using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Mail
{
    public class NullMailDispatchService : IMailDispatchService
    {
        public Guid Send(MailMessage message)
        {
            return System.Guid.NewGuid();
        }

        public Guid Send(MailMessage message, MailOptions options)
        {
            return System.Guid.NewGuid();
        }

        public MailDeliveryInfo GetDeliveryInfoFor(Guid messageId)
        {
            return new MailDeliveryInfo() { };
        }

        public IEnumerable<MailDeliveryInfo> GetDeliveryInfoFor(IEnumerable<Guid> messageIds)
        {
            return new List<MailDeliveryInfo>();
        }
    }
}
