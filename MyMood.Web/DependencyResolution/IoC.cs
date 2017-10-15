using StructureMap;
namespace MyMood.Web {
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
            {

                x.For<Discover.DomainModel.IDomainDataContext>().HybridHttpOrThreadLocalScoped().Use<MyMood.Infrastructure.EntityFramework.MyMoodDbContext>();

                x.For<Discover.Logging.ILogger>().HybridHttpOrThreadLocalScoped().Use<Discover.Logging.NLog.NLoggerWithElmah>();
                x.For<Discover.Mail.IMailDeliveryService>().HybridHttpOrThreadLocalScoped().Use<Discover.Mail.MailBee.MailBeeMailServiceProvider>();
                x.For<Discover.Mail.IMailDispatchService>().HybridHttpOrThreadLocalScoped().Use<Discover.Mail.MailBee.MailBeeMailServiceProvider>();
                x.For<Discover.HtmlTemplates.IHtmlTemplateManager>().Singleton()
                               .Use<Discover.HtmlTemplates.Themed.ThemedHtmlTemplateManager>()
                               .Ctor<string>().Is(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/HtmlTemplates"));

                x.For<IDataSyncClient>().HybridHttpOrThreadLocalScoped().Use<DataSyncAgent>();
            });

            return ObjectFactory.Container;
        }
    }
}