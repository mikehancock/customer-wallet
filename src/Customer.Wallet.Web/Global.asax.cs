namespace Customer.Wallet.Web
{
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using Castle.MicroKernel.Registration;
    using Castle.Windsor;

    using Customer.Wallet.Core;
    using Customer.Wallet.Core.Domain;
    using Customer.Wallet.Storage;
    using Customer.Wallet.Web.Controllers;
    using Customer.Wallet.Web.IoC;

    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Embedded;

    public class WebApiApplication : System.Web.HttpApplication
    {
        private static IWindsorContainer container;

        public override void Dispose()
        {
            container.Dispose();
            base.Dispose();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            container = new WindsorContainer();
            container.Register(
                Component.For<WalletController>().LifestyleTransient(),
                Component.For<IRepository<Account>>().ImplementedBy<AccountRepository>().LifestylePerWebRequest(),
                Component.For<IDocumentSession>().UsingFactoryMethod(
                    () =>
                        {
                            ////var docStore = new EmbeddableDocumentStore() { DataDirectory = "Data", UseEmbeddedHttpServer = true };
                            var docStore = new DocumentStore() { Url = "http://localhost:8080/", DefaultDatabase = "Wallet" };
                            docStore.Initialize();
                            return docStore.OpenSession();
                        }).LifestylePerWebRequest());
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new WindsorCompositionRoot(container));
        }
    }
}
