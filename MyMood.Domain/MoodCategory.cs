using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyMood.Domain
{
    public class MoodCategory : Entity
    {
         protected MoodCategory()
            :base()
        {
            this.MoodsCollection = new List<Mood>();
        }

         public MoodCategory(Event evnt, string name) :
            this()
        {
            this.Event = evnt;
            this.Name = name;
        }

         public MoodCategory(Event evnt, Guid id, string name) :
             this(evnt, name)
         {
             this.Id = id;
         }

         public string Name { get; set; }
         public virtual Event Event { get; set; }

         public IEnumerable<Mood> Moods { get { return MoodsCollection; } }
         protected virtual ICollection<Mood> MoodsCollection { get; set; }

         public Mood AddMood(string name, MoodType moodType, string displayColor, int displayIndex)
         {
             Mood mood = this.MoodsCollection.FirstOrDefault(m => m.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
             if (mood == null)
             {
                 mood = new Mood(this, name, moodType, displayColor,  displayIndex);
                 this.MoodsCollection.Add(mood);
             }
             return mood;
         }
    }
}
