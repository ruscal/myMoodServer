using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMood.Web.Models;
using Discover.DomainModel;
using Discover.Logging;
using MyMood.Domain;
using Discover.Web.Mvc;
using System.Drawing;
using Discover.Mail;
using Discover.HtmlTemplates;


namespace MyMood.Web.Controllers
{
    public partial class CalloutController : EventControllerBase
    {
        public const int _defaultMoodIsStaleMins = 0;
        public const float _defaultTension = 0.5F;
        public const int _defaultMoodMapWidth = 1024;
        public const int _defaultMoodMapHeight = 768;

        public CalloutController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }


        //[ReportPassCodeOrAuthenticationRequired]
        //public virtual ActionResult MoodSnapshot(string EventName, string Orientation, int? MoodIsStaleMins)
        //{
        //    var evnt = GetEvent(EventName);
        //    var moodIsStaleMins = MoodIsStaleMins ?? _defaultMoodIsStaleMins;
        //    var passCode = (string)RouteData.Values["PassCode"];
        //    return View(new MoodSnapshotModel()
        //    {
        //        Orientation = Orientation,
        //        PassCode = passCode,
        //        Data = GetSnapshot(evnt.MoodCategories.FirstOrDefault(), DateTime.UtcNow, moodIsStaleMins, true),
        //        MoodIsStaleMins = moodIsStaleMins
        //    });
        //}


        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult MoodMap(string EventName, GlobalMoodReportRequestModel request)
        {
            try
            {
                var evnt = GetEvent(EventName);
                var categoryName = request.CategoryName ?? "Default";
                var category = evnt.MoodCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));
                var reportData = GetGlobalMoodReport(evnt, category, request.ReportStart ?? evnt.StartDate ?? DateTime.UtcNow.Date, request.ReportEnd ?? DateTime.UtcNow, request.MoodIsStaleMins ?? _defaultMoodIsStaleMins, false, 10);

                reportData.Snapshots.Skip(1).ForEach(s =>
                {
                    s.t = evnt.ConvertFromUTC(s.t).Value;
                });

                return Request.IsAjaxRequest() ?
                    Json(reportData, JsonRequestBehavior.AllowGet) as ActionResult :
                    View(reportData);
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed getting mood map feed"));
                return View();
            }
        }

        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult GetMoodMapImage(string EventName, string Category, DateTime? ReportStart, DateTime? ReportEnd, int? MoodIsStaleMins, float? Tension, bool? ShowDataPoints, int? Width, int? Height)
        {
            try
            {
                var evnt = GetEvent(EventName);
                Category = Category ?? "Default";
                var category = evnt.MoodCategories.FirstOrDefault(c => c.Name.Equals(Category, StringComparison.InvariantCultureIgnoreCase));
                var moodStaleMins = MoodIsStaleMins ?? _defaultMoodIsStaleMins;
                return new ImageResult()
                {
                    Image = GetGlobalMoodImage(evnt, category, ReportStart ?? DateTime.UtcNow.Date, ReportEnd ?? DateTime.UtcNow, moodStaleMins, Tension ?? _defaultTension, ShowDataPoints ?? false, Width ?? _defaultMoodMapWidth, Height ?? _defaultMoodMapHeight),
                    ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg
                };
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed getting global mood report image - from=[{0}] to=[{0}]", ReportStart, ReportEnd));
                return View();
            }
        }



        [ReportPassCodeOrAuthenticationRequired]
        public virtual JsonResult GetGlobalMoodReportData(string EventName, GlobalMoodReportRequestModel request)
        {
            try
            {
                var evnt = GetEvent(EventName);
                var categoryName = request.CategoryName ?? "Default";
                var category = evnt.MoodCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));
                var moodIsStaleMins = request.MoodIsStaleMins ?? _defaultMoodIsStaleMins;
                return Json(GetGlobalMoodReport(evnt, category, request.ReportStart ?? DateTime.UtcNow.Date, request.ReportEnd ?? DateTime.UtcNow.AddDays(1).Date, moodIsStaleMins, false), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (request == null)
                {
                    this.logger.Error(this.GetType(), ex, "Failed getting global mood report - model is null");
                }
                else
                {
                    this.logger.Error(this.GetType(), ex, string.Format("Failed getting global mood report - from=[{0}] to=[{0}]", request.ReportStart, request.ReportEnd));
                }
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
        }

        //[ReportPassCodeOrAuthenticationRequired]
        //public virtual JsonResult GetMoodSnapshotData(string EventName, MoodSnapshotRequestModel request)
        //{
        //    try
        //    {
        //        var evnt = GetEvent(EventName);
        //        var categoryName = request.CategoryName ?? "Default";
        //        var category = evnt.MoodCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));
        //        var moodIsStaleMins = request.MoodIsStaleMins ?? _defaultMoodIsStaleMins;
        //        return Json(GetSnapshot(category, request.TimeOfSnapshot ?? DateTime.UtcNow, moodIsStaleMins, true), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (request == null)
        //        {
        //            this.logger.Error(this.GetType(), ex, "Failed getting mood shapshot - model is null");
        //        }
        //        else
        //        {
        //            this.logger.Error(this.GetType(), ex, string.Format("Failed getting mood shapshot - timeOfSnapshot=[{0}]", request.TimeOfSnapshot));
        //        }
        //        return Json(new { }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult MoodThermometer(string EventName, MoodSnapshotRequestModel request)
        {
            try
            {
                var evnt = GetEvent(EventName);
                var categoryName = request.CategoryName ?? "Default";
                var category = evnt.MoodCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));

                //var responses = GetSnapshotResponses(category, request.TimeOfSnapshot ?? DateTime.UtcNow, request.MoodIsStaleMins ?? _defaultMoodIsStaleMins);

                var snapshot = GetSnapshot(category, request.TimeOfSnapshot ?? DateTime.UtcNow, false);
                var positive = snapshot.d.Where(m => m.i <= 5).Sum(m => m.c);
               // var percentagePositive = responses.Any() ? ((decimal)responses.Where(r => r.Mood.MoodType == MoodType.Positive).Count() / (decimal)responses.Count()) * 100M : 0M;

                var percentagePositive = snapshot.r == 0 ? 50M : ((decimal)positive / (decimal)snapshot.r) * 100M;

                return Request.IsAjaxRequest() ?
                    Json(new { responseCount = snapshot.r, percentagePositive = percentagePositive }, JsonRequestBehavior.AllowGet) as ActionResult :
                    View();
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed getting mood thermometer"));
                return View();
            }
        }

        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult MoodBreakdown(string EventName, MoodSnapshotRequestModel request)
        {
            try
            {
                var evnt = GetEvent(EventName);
                var categoryName = request.CategoryName ?? "Default";
                var category = evnt.MoodCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));
                var snapshot = GetSnapshot(category, request.TimeOfSnapshot ?? DateTime.UtcNow, true, 10);

                return Request.IsAjaxRequest() ?
                    Json(snapshot, JsonRequestBehavior.AllowGet) as ActionResult :
                    View(snapshot);
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed getting mood breakdown"));
                return View();
            }
        }

        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult MoodTower(string EventName, MoodSnapshotRequestModel request)
        {
            try
            {
                var evnt = GetEvent(EventName);
                var categoryName = request.CategoryName ?? "Default";
                var category = evnt.MoodCategories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.InvariantCultureIgnoreCase));
                var snapshot = GetSnapshot(category, request.TimeOfSnapshot ?? DateTime.UtcNow, true, 10);

                return Request.IsAjaxRequest() ?
                    Json(snapshot, JsonRequestBehavior.AllowGet) as ActionResult :
                    View(snapshot);
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed getting mood tower feed"));
                return View();
            }
        }
    }

    
}
