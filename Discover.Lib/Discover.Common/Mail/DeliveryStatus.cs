using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Mail
{
    public enum DeliveryStatus
    {
        Pending = 1,
        Sent = 2,
        Failed = 4,
        Suspended = 8,
        Cancelled = 16
    }
}
