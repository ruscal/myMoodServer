using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Mail
{
    [Serializable]
    public class MailDeliveryInfo
    {
        public Guid MessageId { get; set; }

        public DeliveryStatus DeliveryStatus { get; set; }

        public DateTime? SentOn { get; set; }

        public DateTime? FailedOn { get; set; }

        public int FailureCount { get; set; }

        public string FailureMessage { get; set; }
    }
}
