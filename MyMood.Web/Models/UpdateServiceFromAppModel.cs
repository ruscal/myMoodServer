using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class UpdateServiceFromAppModel : AppRequestModelBase
    {
        public IEnumerable<MoodResponseUpdateModel> Responses { get; set; }

        public DateTime? LastUpdate { get; set; }

        public int ResTotal { get; set; }
    }

}