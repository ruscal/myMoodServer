using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class PushNotification  : Entity
    {
        protected PushNotification()
            : base()
        {
            
        }

        public PushNotification(Event evnt, DateTime sendDate, string message, bool playSound)
        {
            this.Event = evnt;
            this.SendDate = sendDate;
            this.Message = message;
            this.PlaySound = playSound;
        }


        public virtual Event Event { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime SendDateLocal
        {
            get
            {
                return this.Event.ConvertFromUTC(this.SendDate).Value;
            }
        }
        public bool PlaySound { get; set; }
        public bool Sent { get; set; }

        public void ConvertAllToUTC()
        {
            this.SendDate = Event.ConvertToUTC(this.SendDate).Value;
        }
    }
}
