using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using FileUploadProvider;
using FileUploadProvider.LocalDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.Models;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            InitDI();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void InitDI()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutofacWebTypesModule>();
            // Register your MVC controllers. (MvcApplication is the name of the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.Register<Func<string,string>>(a => a.Resolve<UrlHelper>().Content);
            //builder.Register(a => new UrlHelper(a.Resolve<RequestContext>()));
            RegisterFileUploadProvider(builder);
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static void RegisterFileUploadProvider(ContainerBuilder builder)
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/bin");
            if (String.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var assemblyName = String.Format("{0}.dll", ConfigurationManager.AppSettings["IUploadProviderAssembly"]);

            //  Gets all compiled assemblies.
            //  This is particularly useful when extending applications functionality from 3rd parties,
            //  if there are interfaces within the modules.
            var assemblies = Directory.GetFiles(path, assemblyName, SearchOption.TopDirectoryOnly)
                                      .Select(Assembly.LoadFrom);

            foreach (var assembly in assemblies)
            {
                //  Gets the all modules from each assembly to be registered.
                //  Make sure that each module **MUST** have a parameterless constructor.
                var modules = assembly.GetTypes()
                                      .Where(p => typeof(IModule).IsAssignableFrom(p)
                                                  && !p.IsAbstract)
                                      .Select(p => (IModule)Activator.CreateInstance(p));

                //  Regsiters each module.
                foreach (var module in modules)
                {
                    builder.RegisterModule(module);
                }
            }
        }

    }
}
