using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Discover.Security
{
    /// <summary>
    /// This attribute may be used as a global filter in an MVC application to enforce an "secured by default" policy on controllers and actions, by requiring that either the current user is authenticated,
    /// or an action is explicitly marked with the AllowAnonymous attribute. 
    /// </summary>
    public sealed class LogonAuthorizeAttribute : AuthorizeAttribute
    {
        public Action<System.Web.HttpContextBase> _onAuthorizationPassed;

        public LogonAuthorizeAttribute() { }

        public LogonAuthorizeAttribute(Action<System.Web.HttpContextBase> onAuthorizationPassed)
        {
            _onAuthorizationPassed = onAuthorizationPassed;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var skipAuthorisation = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), false);

            if (!skipAuthorisation)
            {
                base.OnAuthorization(filterContext);
            }
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);

            if (_onAuthorizationPassed != null)
            {
                _onAuthorizationPassed(httpContext);
            }

            return isAuthorized;
        }
    }
}
