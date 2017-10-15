using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Discover.Security;
using MyMood.Domain;

namespace MyMood.Web
{
    public abstract class ViewBase<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        protected override void InitializePage()
        {
            base.InitializePage();
            this.ViewBag.SiteName = this.ViewBag.SiteName ?? System.Configuration.ConfigurationManager.AppSettings["SiteName"] ?? string.Empty;
        }

        public new ExtendedPrincipal<User> User
        {
            get { return base.User as ExtendedPrincipal<User>; }
        }
    }
}