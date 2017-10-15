using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace Discover.Theming
{
    public class ThemedRazorViewEngine : RazorViewEngine
    {
        protected override IView CreateView(ControllerContext controllerContext, string viewPath,
                             string masterPath)
        {
            //Check to see if the action requires a theme to be set
            if (!controllerContext.Controller.ViewData.ContainsKey("__renderTheme"))
                return base.CreateView(controllerContext, viewPath, masterPath);

            string themeMaster = controllerContext.Controller.ViewData.ContainsKey("__themeMaster") ? (string)controllerContext.Controller.ViewData["__themeMaster"] : "site";
            string theme = controllerContext.Controller.ViewData.ContainsKey("__theme") ? (string)controllerContext.Controller.ViewData["__theme"] : "Default";
            if (theme == "") theme = "Default";
            //sets the path to the master page which will be manuipulated based on the theme setting
            const string path = "~/Content/Themes/{0}/Views/_{1}.cshtml";
            //manipulate which master page to use...      
            string themeMasterPath = string.Format(path, theme, themeMaster);

            //Check the path exists 
            if (File.Exists(controllerContext.HttpContext.Server.MapPath(themeMasterPath)))
                //call the create view method with the new information set....
                return base.CreateView(controllerContext, viewPath, themeMasterPath);
            //If not theme and master page is found then throw
            throw new FileNotFoundException("A master page cannot be found in the folder "
                                   + themeMasterPath);
        }
    }
}
