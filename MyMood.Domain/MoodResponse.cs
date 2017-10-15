using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class MoodResponse : Entity
    {
        protected MoodResponse()
            :base()
        {

        }

        public MoodResponse(Responder responder, Mood mood, DateTime timeStamp, MoodPrompt prompt)
            :this()
        {
            this.Responder = responder;
            this.Mood = mood;
            this.TimeStamp = timeStamp;
           
            this.TimeStampRounded = GetRoundedResponseTime(timeStamp);
            this.Prompt = prompt;
        }

        public MoodResponse(Guid id, Responder responder, Mood mood, DateTime timeStamp, MoodPrompt prompt)
            : this(responder, mood, timeStamp, prompt)
        {
            this.Id = id;
        }

        public virtual Responder Responder { get; protected set; }
        public virtual Mood Mood { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime TimeStampRounded { get; set; }
        public virtual MoodPrompt Prompt { get; set; }

        public virtual Event Event { get; set; }

        public static DateTime GetRoundedResponseTime(DateTime timeStamp)
        {
            var tenMinth = (int)Math.Ceiling((decimal)timeStamp.Minute / 10M) * 10;
            var diff = tenMinth - timeStamp.Minute;
            var ts = timeStamp.AddMinutes(diff);
            return new DateTime(ts.Year, ts.Month, ts.Day, ts.Hour, ts.Minute, 0, DateTimeKind.Utc);
        }
    }
}
