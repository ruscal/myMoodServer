using System.Web.Mvc;
using StructureMap;

[assembly: WebActivator.PreApplicationStartMethod(typeof(MyMood.Web.App_Start.StructuremapMvc), "Start")]

namespace MyMood.Web.App_Start {
    public static class StructuremapMvc {
        public static void Start() {
            var container = (IContainer) IoC.Initialize();
            DependencyResolver.SetResolver(new SmDependencyResolver(container));
            System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new SmDependencyResolver(container);
        }
    }
}