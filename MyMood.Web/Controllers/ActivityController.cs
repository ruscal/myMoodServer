using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyMood.Web.Models;
using Discover.DomainModel;
using Discover.Logging;
using MyMood.Domain;
using Discover.Mail;
using Discover.HtmlTemplates;
using Discover.Web.Mvc;

namespace MyMood.Web.Controllers
{
    public partial class ActivityController : EventControllerBase
    {
        private Event e;
        private Activity a;

        public ActivityController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {
        }

        public virtual ViewResult Index(string EventName)
        {
            e = GetEvent(EventName);
            ViewBag.Title = String.Format("Independent Activities for: {0}", e.Title);

            var model = e.IndependentActivities.OrderBy(x => x.TimeStamp).Select(x => new ActivityModel()
            {
                Id = x.Id,
                EventId = e.Id,
                Title = x.Title,
                TimeStamp = x.TimeStampLocal, 
                TimeStampUTC = x.TimeStamp.ToWebDateTimeUTC()
            }).ToList();

            return View(MVC.Activity.Views.ActivityIndex, model);
        }

        [HttpGet]
        public virtual ActionResult Add(Guid eventId)
        {
            e = GetEvent(eventId);
            var model = new EditActivityModel
            {
                EventId = e.Id,
                TimeStamp = new ModelTime { Date = e.NowLocal }
            };

            return PartialView(MVC.Activity.Views.Edit, model);
        }


        [HttpGet]
        public virtual ActionResult Edit(string EventName, Guid id)
        {
            try
            {
                e = GetEvent(EventName);
                a = e.IndependentActivities.Where(x => x.Id == id).First();
            }
            catch (Exception)
            {
                throw new HttpException(404, "Activity not found");
            }

            var model = new EditActivityModel()
            {
                Id = a.Id,
                EventId = e.Id,
                Title = a.Title,
                TimeStamp = new ModelTime { Date = a.TimeStampLocal }
            };
            return PartialView(MVC.Activity.Views.Edit, model);
        }


        [HttpPost]
        public virtual ActionResult Save(EditActivityModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    e = GetEvent(model.EventId);

                    if (model.Id == Guid.Empty)
                    {
                        a = e.AddActivity(model.Title, model.TimeStamp.FullDate);
                    }
                    else
                    {
                        a = e.IndependentActivities.Where(x => x.Id == model.Id).First();
                        a.Title = model.Title;
                        a.TimeStamp = model.TimeStamp.FullDate;
                        a.ConvertAllToUTC();
                    }
                   db.SaveChanges();
                }
                catch (Exception ex)
                {
                    this.logger.Error(this.GetType(), ex, "Failed saving activity");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return ModelState.IsValid ?
                Json(new { success = true }) :
                Json(new { success = false, formWithErrorMessages = this.RenderPartialViewToString(MVC.Activity.Views.Edit, model) });
        }

        [HttpPost]
        public virtual ActionResult Delete(Guid id, Guid eventId)
        {
            try
            {
                e = GetEvent(eventId);
                a = e.IndependentActivities.Where(x => x.Id == id).First();
                db.Remove(a);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                this.logger.Error(this.GetType(), ex, string.Format("Failed deleting activity {0}", id));
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
    }
}