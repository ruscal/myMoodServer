using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Discover.Common
{
    public class AuditedObject
    {
        public AuditedObject()
        {
        }

        public AuditedObject(string createdBy, DateTime createdOn, string lastEditedBy, DateTime lastEditedOn)
        {
            CreatedBy = createdBy;
            CreatedOn = createdOn;
            LastEditedBy = lastEditedBy;
            LastEditedOn = lastEditedOn;
        }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastEditedBy { get; set; }
        public DateTime LastEditedOn { get; set; }
    }
}
