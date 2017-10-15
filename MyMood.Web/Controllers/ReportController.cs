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
using Discover.Common;

namespace MyMood.Web.Controllers
{
    public partial class ReportController : EventControllerBase
    {
        public const int _defaultMoodIsStaleMins = 0;
        public const float _defaultTension = 0.5F;
        public const int _defaultMoodMapWidth = 1024;
        public const int _defaultMoodMapHeight = 768;

        public ReportController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {

        }

        public virtual ActionResult Index()
        {
            var responses = this.db.Get<MoodResponse>();

            MoodResponsesViewModel model = new MoodResponsesViewModel()
            {
                Responses = responses.OrderByDescending(r => r.TimeStamp).ThenByDescending(r => r.CreationDate).Take(500).Select(r =>
                    new MoodResponseViewModel()
                    {
                        ResponderId = r.Responder.Id,
                        Mood = r.Mood.Name,
                        TimeStamp = r.TimeStamp,
                        PromptText = r.Prompt == null ? "---" : r.Prompt.NotificationText
                    }).ToList()
            };

            return View(model);
        }

        //[HttpPost]
        //public ActionResult Index(MoodSnapshotRequestModel request)
        //{
        //    var moodIsStaleMins = request.MoodIsStaleMins ?? _defaultMoodIsStaleMins;
        //    var responses = request == null ? this.db.Get<MoodResponse>() : GetSnapshotResponses(request.CategoryName ?? "Default", request.TimeOfSnapshot ?? DateTime.UtcNow, moodIsStaleMins);

        //    MoodResponsesViewModel model = new MoodResponsesViewModel()
        //    {
        //        Responses = responses.OrderByDescending(r => r.TimeStamp).ThenByDescending(r => r.CreationDate).Take(500).Select(r =>
        //            new MoodResponseViewModel()
        //            {
        //                ResponderId = r.Responder.Id,
        //                Mood = r.Mood.Name,
        //                TimeStamp = r.TimeStamp,
        //                PromptText = r.Prompt == null ? "---" : r.Prompt.NotificationText
        //            }).ToList()
        //    };

        //    return View(model);
        //}

        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult PersonalMoodReport(string EventName, Guid responderId)
        {
            try
            {
                Event evnt = GetEvent(EventName);

                if (responderId == null) throw new ArgumentException("Invalid responder");

                Responder responder = this.db.Get<Responder>().FirstOrDefault(r => r.Id == responderId && r.Event.Id == evnt.Id);
 
               
                return View(new PersonalMoodReportViewModel()
                {
                    ResponderId = responderId.ToString(),
                    Responses = responder.Responses.OrderBy(r => r.TimeStamp).Select(r => new PersonalMoodResponse()
                    {
                        Mood = r.Mood.Name,
                        TimeStamp = r.TimeStamp.ToLocalTime(evnt.ApplicationConfig.TimeZone),
                        Title = r.Prompt == null ? "My Mood" : r.Prompt.Activity.Title
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed generating personal report - responder=[{0}]  error=[{1}]", responderId,  ex.Message));
                return View(new PersonalMoodReportViewModel()
                {
                    ResponderId = responderId.ToString(),
                    Responses = new List<PersonalMoodResponse>()
                });
            }
        }

        [ReportPassCodeOrAuthenticationRequired]
        public virtual ActionResult PersonalMoodReportPdf(string EventName, Guid responderId)
        {
            Event evnt = GetEvent(EventName);

            if (responderId == null) throw new ArgumentException("Invalid responder");

            Responder responder = this.db.Get<Responder>().FirstOrDefault(r => r.Id == responderId && r.Event.Id == evnt.Id);
 
                return File(ReportHelper.PersonalMoodReportBytes(EventName, responderId, evnt.ApplicationConfig.ReportPassCode), "application/octet", "myMoodReport.pdf");
        }

        //[TokenAuthenticationRequired]
        //public ActionResult MoodSnapshot(string Orientation, int? MoodIsStaleMins)
        //{
        //    var moodIsStaleMins = MoodIsStaleMins ?? _defaultMoodIsStaleMins;
        //    var passCode = (string)RouteData.Values["PassCode"];
        //    return View(new MoodSnapshotModel()
        //    {
        //        Orientation = Orientation,
        //        PassCode = passCode,
        //        Data = GetSnapshot("Default", DateTime.UtcNow, moodIsStaleMins),
        //        MoodIsStaleMins = moodIsStaleMins
        //    });
        //}

        //[HttpGet]
        //[TokenAuthenticationRequired]
        //public JsonResult GetMoodSnapshotData(MoodSnapshotRequestModel model)
        //{
        //    try
        //    {
        //        if (model == null) model = new MoodSnapshotRequestModel(){
        //            CategoryName = "Default",
        //            TimeOfSnapshot = DateTime.UtcNow                    
        //        };
        //        return Json(GetSnapshot(model.CategoryName, model.TimeOfSnapshot));
        //    }
        //    catch (Exception ex)
        //    {
        //        if (model == null)
        //        {
        //            this.logger.Error(this.GetType(), ex, "Failed getting mood shapshot - model is null");
        //        }
        //        else
        //        {
        //            this.logger.Error(this.GetType(), ex, string.Format("Failed getting mood shapshot - timeOfSnapshot=[{0}]", model.TimeOfSnapshot));
        //        }
        //        return Json(new { });
        //    }
        //}


        //[TokenAuthenticationRequired]
        //public ActionResult MoodMap()
        //{
        //    return View();
        //}

        //[TokenAuthenticationRequired]
        //public ActionResult GetMoodMapImage(string Category, DateTime? ReportStart, DateTime? ReportEnd, int? MoodIsStaleMins, float? Tension, bool? ShowDataPoints, int? Width, int? Height)
        //{
        //    try
        //    {
        //        var moodStaleMins = MoodIsStaleMins ?? _defaultMoodIsStaleMins;
        //        return new ImageResult()
        //        {
        //            Image = GetGlobalMoodImage(Category ?? "Default", ReportStart ?? DateTime.UtcNow.Date, ReportEnd ?? DateTime.UtcNow, moodStaleMins, Tension ?? _defaultTension, ShowDataPoints ?? false, Width ?? _defaultMoodMapWidth, Height ?? _defaultMoodMapHeight),
        //            ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.Error(this.GetType(), ex, string.Format("Failed getting global mood report image - from=[{0}] to=[{0}]", ReportStart, ReportEnd));
        //        return new ImageResult();
        //    }
        //}



        //[TokenAuthenticationRequired]
        //public JsonResult GetGlobalMoodReportData(GlobalMoodReportRequestModel request)
        //{
        //    try
        //    {
        //        var moodIsStaleMins = request.MoodIsStaleMins ?? _defaultMoodIsStaleMins;
        //        return Json(GetGlobalMoodReport(request.CategoryName ?? "Default", request.ReportStart ?? DateTime.UtcNow.Date, request.ReportEnd ?? DateTime.UtcNow.AddDays(1).Date, moodIsStaleMins), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (request == null)
        //        {
        //            this.logger.Error(this.GetType(), ex, "Failed getting global mood report - model is null");
        //        }
        //        else
        //        {
        //            this.logger.Error(this.GetType(), ex, string.Format("Failed getting global mood report - from=[{0}] to=[{0}]", request.ReportStart, request.ReportEnd));
        //        }
        //        return Json(new { }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        //[TokenAuthenticationRequired]
        //public JsonResult GetMoodSnapshotData(MoodSnapshotRequestModel request)
        //{
        //    try
        //    {
        //        var moodIsStaleMins = request.MoodIsStaleMins ?? _defaultMoodIsStaleMins;
        //        return Json(GetSnapshot(request.CategoryName ?? "Default", request.TimeOfSnapshot ?? DateTime.UtcNow, moodIsStaleMins), JsonRequestBehavior.AllowGet);
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

        //public ActionResult Test()
        //{
        //    var snaphot = GetSnapshot("Default", DateTime.Now, 0);

        //    return View();
        //}

        

    }
}
