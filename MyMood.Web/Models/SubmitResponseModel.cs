using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyMood.Web.Models
{
    public class SubmitResponseModel : AppRequestModelBase
    {
        public SubmitResponseModel()
        {

        }

        public MoodResponseUpdateModel r { get; set; }
    }
}