using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class MoodServer : Entity
    {
        protected MoodServer()
            : base()
        {
        }

        public MoodServer(string name)
            : this()
        {
            Name = name;
        }

        public string Name { get; protected set; }

        public MoodServerType ServerType
        {
            get { return (MoodServerType)ServerTypeEnumValue; }
            set { ServerTypeEnumValue = (int)value; }
        }
        protected int ServerTypeEnumValue { get; set; }

        public bool CanPushClientNotifcations { get; set; }

        public string ApiKey { get; set; }

        public string BaseAddress { get; set; }

        public DateTime? LastSuccessfulSync { get; set; }
    }

    public enum MoodServerType
    {
        WAN = 0,
        LAN = 1
    }
}
