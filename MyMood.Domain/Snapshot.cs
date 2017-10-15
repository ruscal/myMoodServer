using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class Snapshot : Entity
    {
        protected Snapshot()
            : base()
        {
            
        }

        public virtual Mood Mood { get; set; }
        public DateTime TimeStamp { get; set; }
        public int ResponseCount { get; set; }
        public virtual MoodCategory Category { get; set; }

    }
}
