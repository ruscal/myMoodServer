using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Discover.Logging;

namespace Discover.Web.Mvc
{
    public class RequireBrowserCapabilitiesAttribute : ActionFilterAttribute
    {
        #region Static Members

        /// <summary>
        /// The default URL to redirect to if the client browser does not satifisy the specified version/capability requirements.
        /// </summary>
        public static string DefaultRedirectUrl { get; set; }

        #endregion

        /// <summary>
        /// A string containing a pipe-separated set of user-agent fragments which should be denied
        /// </summary>
        public string DenyAgents { get; set; }

        /// <summary>
        /// The URL to redirect to if the client browser does not satifisy the specified version/capability requirements.
        /// </summary>
        public string RedirectUrl
        {
            get { return redirectUrl; }
            set { redirectUrl = value; }
        }

        protected Func<HttpBrowserCapabilitiesBase, bool> Predicate { get; set; }

        private string redirectUrl = DefaultRedirectUrl;

        public RequireBrowserCapabilitiesAttribute(string denyAgents)
        {
            DenyAgents = denyAgents;
        }

        public RequireBrowserCapabilitiesAttribute(Func<HttpBrowserCapabilitiesBase, bool> predicate)
        {
            Predicate = predicate;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var browserCaps = filterContext.HttpContext.Request.Browser;
            var userAgentString = filterContext.HttpContext.Request.UserAgent.ToLowerInvariant();
            var failureRedirectUrl = RedirectUrl ?? DefaultRedirectUrl;

            if (!filterContext.HttpContext.Request.AppRelativeCurrentExecutionFilePath.Equals(failureRedirectUrl, StringComparison.InvariantCultureIgnoreCase))
            {
                if ((Predicate != null && !Predicate(browserCaps)) ||
                    (!string.IsNullOrWhiteSpace(DenyAgents) && DenyAgents.Split('|').Any(s => userAgentString.Contains(s.ToLowerInvariant()))))
                {
                    filterContext.Result = new RedirectResult(failureRedirectUrl);
                    ILogger logger = DependencyResolver.Current.GetService<Discover.Logging.ILogger>();
                    logger.Warn("BROWSER_CAPABILITY", string.Format("Could not load resource [{0}] - browser is not of required version  [{1}]", filterContext.HttpContext.Request.RawUrl, filterContext.HttpContext.Request.UserAgent));
                }
            }
            
            base.OnActionExecuting(filterContext);
        }
    }
}
