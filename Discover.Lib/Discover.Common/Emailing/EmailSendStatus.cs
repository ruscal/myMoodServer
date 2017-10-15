using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Emailing
{
    [Obsolete("Please use classes and interfaces in Discover.Mail namespace for all new projects")]
    public enum EmailSendStatus
    {
        PENDING = 1,
        SENT = 2,
        FAILED = 4,
        SUSPENDED = 8,
        CANCELLED = 16
    }
}
