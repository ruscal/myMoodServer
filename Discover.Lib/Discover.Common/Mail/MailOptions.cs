using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Mail
{
    [Serializable]
    public class MailOptions
    {
        /// <summary>
        /// Indicates whether an email should be digitally signed before sending
        /// </summary>
        public bool Sign { get; set; }

        /// <summary>
        /// Indicates whether an email should be encrypted before sending
        /// </summary>
        public bool Encrypt { get; set; }

        /// <summary>
        /// Indicates whether an email should be held in a suspended state (rather than sent ASAP)
        /// </summary>
        public bool Suspend { get; set; }

        /// <summary>
        /// Indicates that the sending of an email should be deferred until after the given date and time
        /// </summary>
        public DateTime? DelayUntil { get; set; }
    }
}
