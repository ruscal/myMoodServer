using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Discover.DomainModel;
using Discover.Logging;
using MyMood.Domain;
using System.Web.Configuration;
using Discover.Mail;
using Discover.HtmlTemplates;
using Discover.Security;

namespace MyMood.Web.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected IDomainDataContext db;
        protected ILogger logger;
        protected IMailDispatchService mailer;
        protected IHtmlTemplateManager htmlTemplateManager;

        protected ControllerBase()
        {
        }

        protected ControllerBase(IDomainDataContext db, ILogger logger, IMailDispatchService mailer, IHtmlTemplateManager htmlTemplateManager)
        {
            this.db = db;
            this.logger = logger;
            this.mailer = mailer;
            this.htmlTemplateManager = htmlTemplateManager;
        }

        public MoodServer MoodServer
        {
            get
            {
                string serverName = Configuration.WebConfiguration.ServerName ?? "Default";
                var server = this.db.Get<MoodServer>().FirstOrDefault(s => s.Name.Equals(serverName, StringComparison.InvariantCultureIgnoreCase));
                if (server == null)
                {
                    server = new MoodServer(serverName);
                    this.db.Add(server);
                    this.db.SaveChanges();
                }
                return server;
            }
        }


        protected new void ValidateRequest(string passCode)
        {
            var validId = Configuration.WebConfiguration.ApplicationPassCode;
            if (!validId.Equals(passCode, StringComparison.InvariantCultureIgnoreCase)) throw new UnauthorizedAccessException(string.Format("Could not validate request id [{0}]", passCode));
        }

        public new ExtendedPrincipal<User> User
        {
            get { return base.User as ExtendedPrincipal<User>; }
        }

        protected PartialViewResult PartialView(string viewName, object model, string htmlFieldPrefix)
        {
            var result = this.PartialView(viewName, model);

            if (!string.IsNullOrEmpty(htmlFieldPrefix))
            {
                result.ViewData.TemplateInfo = new TemplateInfo()
                {
                    HtmlFieldPrefix = string.IsNullOrEmpty(result.ViewData.TemplateInfo.HtmlFieldPrefix) ? htmlFieldPrefix : string.Concat(result.ViewData.TemplateInfo.HtmlFieldPrefix, ".", htmlFieldPrefix)
                };
            }

            return result;
        }

    }
}