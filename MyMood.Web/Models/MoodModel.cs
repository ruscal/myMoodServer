using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyMood.Domain;

namespace MyMood.Web.Models
{
    public class MoodModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public int DisplayIndex { get; set; }
        public string DisplayColor { get; set; }
        public MoodType MoodType { get; set; }
    }
}