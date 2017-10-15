using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class MoodServerSyncReport : Entity
    {
        protected MoodServerSyncReport()
            : base()
        {
        }

        public MoodServerSyncReport(MoodServer initiatingServer, MoodServer targetServer,  DateTime timeStamp)
            : this()
        {
            this.InitiatingServer = initiatingServer;
            this.TargetServer = targetServer;
            this.TimeStamp = timeStamp;
        }

        public MoodServerSyncReport(Guid logId, MoodServer initiatingServer, MoodServer targetServer, DateTime timeStamp)
            : this(initiatingServer, targetServer, timeStamp)
        {
            this.Id = logId;
        }

        public virtual MoodServer InitiatingServer { get; protected set; }
        public virtual MoodServer TargetServer { get; protected set; }
        public DateTime TimeStamp { get; protected set; }
    }
}
