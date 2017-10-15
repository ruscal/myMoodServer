using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class Mood :Entity
    {
        protected Mood()
            :base()
        {
            this.ResponsesCollection = new List<MoodResponse>();
        }

        public Mood(MoodCategory category, string name, MoodType moodType, string displayColor, int displayIndex) :
            this()
        {
            this.Category = category;
            this.Name = name;
            this.DisplayColor = displayColor;
            this.DisplayIndex = displayIndex;
            this.MoodType = moodType;
        }

        public string Name { get; set; }
        public int DisplayIndex { get; set; }
        public virtual MoodCategory Category { get; set; }
        public string DisplayColor { get; set; }



        public IEnumerable<MoodResponse> Responses { get { return ResponsesCollection; } }
        protected virtual ICollection<MoodResponse> ResponsesCollection { get; set; }

        public MoodType MoodType
        {
            get { return (MoodType)MoodTypeEnumValue; }
            protected set { MoodTypeEnumValue = (int)value; }
        }
        private int MoodTypeEnumValue { get; set; }

    }

    public enum MoodType
    {
        Positive = 1,
        Negative = 2,
        Neutral = 4
    }
}
