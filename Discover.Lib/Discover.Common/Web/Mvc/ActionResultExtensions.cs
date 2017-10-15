using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Discover.Web.Mvc
{
    public static class ActionResultExtensions
    {
        /// <summary>
        /// Creates a cookie containing a "flash message" to be shown on the client-side (after following the redirect)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static RedirectResult WithFlashMessage(this RedirectResult result, string message)
        {
            return WithFlashMessage<RedirectResult>(result, null, message);
        }

        /// <summary>
        /// Creates a cookie containing a "flash message" to be shown on the client-side (after following the redirect)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="messageClass"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static RedirectResult WithFlashMessage(this RedirectResult result, string messageClass, string message)
        {
            return WithFlashMessage<RedirectResult>(result, messageClass, message);
        }

        /// <summary>
        /// Creates a cookie containing a "flash message" to be shown on the client-side (after following the redirect)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static RedirectToRouteResult WithFlashMessage(this RedirectToRouteResult result, string message)
        {
            return WithFlashMessage<RedirectToRouteResult>(result, null, message);
        }

        /// <summary>
        /// Creates a cookie containing a "flash message" to be shown on the client-side (after following the redirect)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="messageClass"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static RedirectToRouteResult WithFlashMessage(this RedirectToRouteResult result, string messageClass, string message)
        {
            return WithFlashMessage<RedirectToRouteResult>(result, messageClass, message);
        }

        private static TActionResult WithFlashMessage<TActionResult>(this TActionResult result, string messageClass, string message)
        {
            HttpContext.Current.Response.Cookies.Add(new HttpCookie("FlashMessage", string.Concat(messageClass, "|", message)) { Path = "/" });
            return result;
        }
    }
}
