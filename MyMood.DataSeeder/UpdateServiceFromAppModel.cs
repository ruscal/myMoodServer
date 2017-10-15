using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Models
{
    public class UpdateServiceFromAppModel : AppRequestModelBase
    {
        public IEnumerable<MoodResponseUpdateModel> Responses { get; set; }

        public DateTime? LastSuccessfulServiceUpdate { get; set; }
    }

    public class AppRequestModelBase
    {
        public string AppId { get; set; }
        public string ResponderId { get; set; }
    }

    public class MoodResponseUpdateModel
    {
        public string Id { get; set; }
        public MoodModel MyMood { get; set; }
        public DateTime TimeStamp { get; set; }
        public string MoodPromptId { get; set; }
    }

    public class MoodModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}