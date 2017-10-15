using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Discover.Security
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        #region Static Members

        /// <summary>
        /// An empty array that indicates that any role may access a given action method.
        /// </summary>
        public static readonly string[] Any = new string[0];

        /// <summary>
        /// The default URL to redirect to if authorization fails.
        /// </summary>
        public static string DefaultRedirectUrl { get; set; }

        static AuthorizeRolesAttribute()
        {
            DefaultRedirectUrl = FormsAuthentication.LoginUrl;
        }

        #endregion

        private string[] rolesArray = Any;
        private string redirectUrl = DefaultRedirectUrl;

        /// <summary>
        /// The set of roles allowed to call this action method.
        /// </summary>
        public string[] RolesArray
        {
            get { return rolesArray; }
            set { rolesArray = value; }
        }

        /// <summary>
        /// The URL to redirect to if authorization fails.
        /// </summary>
        public string RedirectUrl
        {
            get { return redirectUrl; }
            set { redirectUrl = value; }
        }

        public AuthorizeRolesAttribute()
        {
        }

        public AuthorizeRolesAttribute(params string[] rolesArray)
        {
            this.RolesArray = rolesArray;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                if (!filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    filterContext.Result = new RedirectResult(this.RedirectUrl);
                }
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.User.Identity.IsAuthenticated && (this.RolesArray.Length == 0 || this.RolesArray.Any(httpContext.User.IsInRole));
        }
    }
}
