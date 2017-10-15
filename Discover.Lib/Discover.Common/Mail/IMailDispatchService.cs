using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;

namespace Discover.Mail
{
    /// <summary>
    /// Provides services for sending (and querying the delivery status of) email messages.
    /// </summary>
    public interface IMailDispatchService
    {
        Guid Send(MailMessage message);

        Guid Send(MailMessage message, MailOptions options);

        MailDeliveryInfo GetDeliveryInfoFor(Guid messageId);

        IEnumerable<MailDeliveryInfo> GetDeliveryInfoFor(IEnumerable<Guid> messageIds);
    }
}
