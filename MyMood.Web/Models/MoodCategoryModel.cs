using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
	public class MoodCategoryModel
	{
		public string Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public IEnumerable<MoodModel> Moods {
			get;
			set;
		}
	}
}

