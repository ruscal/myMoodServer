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
    public partial class PushNotificationController : EventControllerBase
    {
        private Event e;
        private PushNotification pn;

        public PushNotificationController(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
            : base(db, logger, mailer, htmlTemplateManager)
        {
        }

        public virtual ViewResult Index(string EventName)
        {
            e = GetEvent(EventName);
            ViewBag.Title = String.Format("Push Notifications for: {0}", e.Title);

            var model = e.PushNotifications.OrderBy(x => x.SendDate).Select(x => new EditPushNotificationModel()
            {
                Id = x.Id,
                EventId = e.Id,
                Message = (x.Message.Length > 150) ? x.Message.Substring(0, 150) + "..." : x.Message,
                SendDate = new ModelTime { Date = x.SendDateLocal },
                SendDateUTC = x.SendDate.ToWebDateTimeUTC(),
                PlaySound = x.PlaySound,
                Sent = x.Sent
            }).ToList();

            return View(MVC.PushNotification.Views.PushNotificationIndex, model);
        }

        [HttpGet]
        public virtual ActionResult Add(Guid eventId)
        {
            e = GetEvent(eventId);
            var model = new EditPushNotificationModel
            {
                EventId = eventId,
                SendImmediately = true,
                SendDate = new ModelTime { Date = e.NowLocal }
            };

            return PartialView(MVC.PushNotification.Views.Edit, model);
        }


        [HttpGet]
        public virtual ActionResult Edit(string EventName, Guid id)
        {
            try
            {
                e = GetEvent(EventName);
                pn = e.PushNotifications.Where(x => x.Id == id).First();
            }
            catch (Exception)
            {
                throw new HttpException(404, "Push Notification not found");
            }

            var model = new EditPushNotificationModel()
            {
                Id = pn.Id,
                EventId = e.Id,
                Message = pn.Message,
                SendDate = new ModelTime { Date = pn.SendDate },
                PlaySound = pn.PlaySound
            };
            return PartialView(MVC.PushNotification.Views.Edit, model);
        }


        [HttpPost]
        public virtual ActionResult Save(EditPushNotificationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    e = GetEvent(model.EventId);

                    if (model.Id == Guid.Empty)
                    {
                        pn = e.AddPushNotification((model.SendImmediately) ? e.NowLocal : model.SendDate.FullDate,
                                                   model.Message, 
                                                   model.PlaySound);
                    }
                    else
                    {
                        pn = e.PushNotifications.Where(x => x.Id == model.Id).First();
                        if (pn.Sent)
                            throw new Exception("Can't edit Push Notifications already sent");

                        pn.SendDate = (model.SendImmediately) ? e.NowLocal : model.SendDate.FullDate;
                        pn.Message = model.Message;
                        pn.PlaySound = model.PlaySound;
                        pn.ConvertAllToUTC();
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return ModelState.IsValid ?
                Json(new { success = true }) :
                Json(new { success = false, formWithErrorMessages = this.RenderPartialViewToString(MVC.PushNotification.Views.Edit, model) });
        }

        [HttpPost]
        public virtual ActionResult Delete(Guid id, Guid eventId)
        {
            try
            {
                e = GetEvent(eventId);
                pn = e.PushNotifications.Where(x => x.Id == id).First();
                if (pn.Sent)
                    throw new Exception("Can't delete Push Notifications already sent");

                db.Remove(pn);
                db.SaveChanges();
                return Json(new { success = true });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }
    }
}