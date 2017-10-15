using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Discover.Security;
using Discover.DomainModel;
using MyMood.Domain;
using MyMood.Web;

namespace MyMood.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new PasscodeOrAuthenticationRequired(ctx =>
            {
                var db = DependencyResolver.Current.GetService<IDomainDataContext>();
                var user = db.Get<User>().Where(u => u.UserName == HttpContext.Current.User.Identity.Name).FirstOrDefault();
                if (user != null)
                {
                    ctx.User = new ExtendedPrincipal<User>(new ExtendedIdentity<User>(user, u => u.UserName), user.Roles);
                }

            }
            ));
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");
            routes.IgnoreRoute("robots.txt");

            routes.MapRoute("Error", "Error/{id}", new { controller = "Home", action = "Error", id = UrlParameter.Optional });
            routes.MapRoute("LogIn", "LogIn", new { controller = "User", action = "LogIn", returnUrl = UrlParameter.Optional });
            routes.MapRoute("LogOut", "LogOut", new { controller = "User", action = "LogOut" });
            routes.MapRoute("ForgottenPassword", "ForgottenPassword", new { controller = "User", action = "ForgottenPassword" });
            routes.MapRoute("ManageUsers", "Users", new { controller = "User", action = "ManageUsers" });
            routes.MapRoute("AddUser", "Users/Add", new { controller = "User", action = "AddUser" });
            routes.MapRoute("EditUser", "Users/Edit/{userId}", new { controller = "User", action = "EditUser" });
            routes.MapRoute("RemoveUser", "Users/Remove/{userId}", new { controller = "User", action = "RemoveUser" });
            routes.MapRoute("UnlockUser", "Users/Unlock/{userId}", new { controller = "User", action = "UnlockUser" });

            //app web services
            routes.MapRoute("App-Install", "App/{EventName}/Install", new { controller = "App", action = "Install"});
            routes.MapRoute("App-Manifest", "App/{EventName}/Manifest", new { controller = "App", action = "Manifest" });
            routes.MapRoute("App-Install-OurMood", "App/{EventName}/OurMood/Install", new { controller = "App", action = "OurMoodInstall" });
            routes.MapRoute("App-Manifest-OurMood", "App/{EventName}/OurMood/Manifest", new { controller = "App", action = "OurMoodManifest" });
            routes.MapRoute("App", "App/{EventName}/{action}/{PassCode}",  new { controller = "App", action = "Index", PassCode = UrlParameter.Optional });

            //event
            routes.MapRoute("Events", "Events/{action}", new { controller = "Event", Action= "Index" });
            routes.MapRoute("Event-Prompts", "Event/{EventName}/MoodPrompts/", new { controller = "MoodPrompt", action = "Index" });
            routes.MapRoute("Event-PushNotifications", "Event/{EventName}/PushNotifications/", new { controller = "PushNotification", action = "Index" });
            routes.MapRoute("Event-Activities", "Event/{EventName}/Activities/", new { controller = "Activity", action = "Index" });
            routes.MapRoute("Event-Monitor", "Event/{EventName}", new { controller = "Event", action = "MonitorByName" });

            //moodprompt
            routes.MapRoute("MoodPrompt-Add", "Event/{EventName}/MoodPrompts/Add", new { controller = "MoodPrompt", action = "Add" });
            routes.MapRoute("MoodPrompt-Edit", "Event/{EventName}/MoodPrompts/Edit/{id}", new { controller = "MoodPrompt", action = "Edit" });
            routes.MapRoute("MoodPrompt-Delete", "Event/{EventName}/MoodPrompts/Delete/{id}", new { controller = "MoodPrompt", action = "Delete" });
            routes.MapRoute("MoodPrompt", "Event/{EventName}/MoodPrompts/{action}/{id}", new { controller = "MoodPrompt" });

            //Push Notifications
            routes.MapRoute("PushNotification-Add", "Event/{EventName}/PushNotifications/Add", new { controller = "PushNotification", action = "Add" });
            routes.MapRoute("PushNotification-Edit", "Event/{EventName}/PushNotifications/Edit/{id}", new { controller = "PushNotification", action = "Edit" });
            routes.MapRoute("PushNotification-Delete", "Event/{EventName}/PushNotifications/Delete/{id}", new { controller = "PushNotification", action = "Delete" });
            routes.MapRoute("PushNotification", "Event/{EventName}/PushNotifications/{action}/{id}", new { controller = "PushNotification" });

            //Independent Activities
            routes.MapRoute("Activity-Add", "Event/{EventName}/Activities/Add", new { controller = "Activity", action = "Add" });
            routes.MapRoute("Activity-Edit", "Event/{EventName}/Activities/Edit/{id}", new { controller = "Activity", action = "Edit" });
            routes.MapRoute("Activity-Delete", "Event/{EventName}/Activities/Delete/{id}", new { controller = "Activity", action = "Delete" });
            routes.MapRoute("Activity", "Event/{EventName}/Activities/{action}/{id}", new { controller = "Activity" });


            //responder
            routes.MapRoute("Responder-MoodReportPdf", "Responder/MoodReport/{ResponderId}/{PassCode}/Pdf", new { controller = "Responder", action = "PersonalMoodReportPdf", PassCode = UrlParameter.Optional });
            routes.MapRoute("Responder-MoodReport", "Responder/MoodReport/{ResponderId}/{PassCode}", new { controller = "Responder", action = "PersonalMoodReport", PassCode = UrlParameter.Optional });


            //callouts
            routes.MapRoute("Callout-MoodSnapshot", "Callout/{EventName}/MoodSnapshot/{PassCode}", new { controller = "Callout", action = "MoodSnapshot", PassCode = UrlParameter.Optional });
            routes.MapRoute("Callout-MoodMap", "Callout/{EventName}/MoodMap/{PassCode}", new { controller = "Callout", action = "MoodMap", PassCode = UrlParameter.Optional });
            routes.MapRoute("Callout-MoodMapImage", "Callout/{EventName}/GetMoodMapImage/{PassCode}", new { controller = "Callout", action = "GetMoodMapImage", PassCode = UrlParameter.Optional });  
            routes.MapRoute("Callout-MoodBreakdown", "Callout/{EventName}/MoodBreakdown/{PassCode}", new { controller = "Callout", action = "MoodBreakdown", PassCode = UrlParameter.Optional });
            routes.MapRoute("Callout-MoodThermometer", "Callout/{EventName}/MoodThermometer/{PassCode}", new { controller = "Callout", action = "MoodThermometer", PassCode = UrlParameter.Optional });
            routes.MapRoute("Callout-MoodTower", "Callout/{EventName}/MoodTower/{PassCode}", new { controller = "Callout", action = "MoodTower", PassCode = UrlParameter.Optional });
            
            //reports
            routes.MapRoute("MoodSnapshot", "Report/MoodSnapshot/{Orientation}/{PassCode}", new { controller = "Report", action = "MoodSnapshot", PassCode = UrlParameter.Optional });
            routes.MapRoute("MoodMap", "Report/MoodMap/{PassCode}", new { controller = "Report", action = "MoodMap", PassCode = UrlParameter.Optional });
            routes.MapRoute("MoodMapImage", "Report/GetMoodMapImage/{PassCode}", new { controller = "Report", action = "GetMoodMapImage", PassCode = UrlParameter.Optional });
            routes.MapRoute("PersonalMoodReportPdf", "Event/{EventName}/Report/PersonalMoodReport/{responderId}/{PassCode}/Pdf", new { controller = "Report", action = "PersonalMoodReportPdf", PassCode = UrlParameter.Optional });
            routes.MapRoute("PersonalMoodReport", "Event/{EventName}/Report/PersonalMoodReport/{responderId}/{PassCode}", new { controller = "Report", action = "PersonalMoodReport", PassCode = UrlParameter.Optional });
            routes.MapRoute("Report", "Report/{action}/{PassCode}",  new { controller = "Report", action = "Index", PassCode = UrlParameter.Optional });

            routes.MapRoute("Default", "{controller}/{action}/{id}",  new { controller = "Home", action = "Index", id = UrlParameter.Optional } );

        }

        public static void RegisterWebApiRoutes(System.Web.Http.HttpRouteCollection routes)
        {
            routes.MapHttpRoute(
                name: "ApiAction",
                routeTemplate: "api/{controller}/{action}",
                defaults: new { action = "Get" }
            );
        }

        protected void Application_Start()
        {
            MyMood.Infrastructure.EntityFramework.MyMoodDbContext.InitializeDatabase(true);
            ControllerBuilder.Current.SetControllerFactory(new SmControllerFactory());

            AreaRegistration.RegisterAllAreas();

            RegisterWebApiRoutes(System.Web.Http.GlobalConfiguration.Configuration.Routes);
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}