using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Discover.Logging;
using MyMood.Web.Models;


namespace MyMood.Web.Controllers
{
    public partial class EventLogController : ControllerBase
    {

        public EventLogController(ILogger logger)
        {
            this.logger = logger;
        }

        [AcceptVerbs("Get", "Post")]
        public virtual ActionResult Index(ViewEventLogModel model, DateTime? searchLogEarliestDate, DateTime? searchLogLatestDate = null)
        {

            model.EarliestDate.Date = model.EarliestDate.Date ?? searchLogEarliestDate ?? DateTime.UtcNow.AddDays(-2);
            model.LatestDate.Date = model.LatestDate.Date ?? searchLogLatestDate ?? DateTime.UtcNow.AddHours(1);

            var loglist = this.logger.FindLogs(model.EarliestDate.FullDate, 
                                               model.LatestDate.FullDate, 
                                               (string.IsNullOrWhiteSpace(model.ErrorLevel) || model.ErrorLevel == "All") ? string.Empty : model.ErrorLevel,
                                               string.Empty, 
                                               string.IsNullOrWhiteSpace(model.SearchText) ? string.Empty : model.SearchText.Trim(), 
                                               string.Empty, 
                                               string.Empty);
            
            model.EventLogEntries = loglist.OrderByDescending(x => x.TimeStamp)
                                           .Select(x => new EventLogEntryModel { Id = x.Id, 
                                                                                 Level = x.Level, 
                                                                                 Message = x.Message, 
                                                                                 TimeStamp = x.TimeStamp.ToWebDateTime() , 
                                                                                 Source = x.Logger
                                                                               }).ToList();

            //Put this back in if you want cookies to remember your last 
    //        Response.Cookies.Add(new HttpCookie("searchLogEarliestDate") { Path = "/", Value = model.EarliestDate.FullDate.ToWebDateTime() });
    //        Response.Cookies.Add(new HttpCookie("searchLoLatestDate") { Path = "/", Value = model.LatestDate.FullDate.ToWebDateTime() });

            if (Request.IsAjaxRequest())
                return PartialView(MVC.EventLog.Views.EventLogIndex, model);

            return View(MVC.EventLog.Views.EventLogIndex, model);
        }

    }
}
