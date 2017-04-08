using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;
using StockHistory.Logic;

namespace StockHistory
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();

			// use attribute routing
			config.MapHttpAttributeRoutes();

			// use json response and correct Content-type
			config.Formatters.JsonFormatter.MediaTypeMappings
				.Add(new RequestHeaderMapping("Accept",
					"text/html",
					StringComparison.InvariantCultureIgnoreCase,
					true,
					"application/json"));

			var builder = new ContainerBuilder();
			var assemblies = (from file in Directory.GetFiles(GetAppBaseDirectory())
								where (
									(Path.GetExtension(file) == ".dll")
									)
								select Assembly.LoadFrom(file)).ToArray();
			builder.RegisterApiControllers(assemblies);
			// register from IoC.cs
			builder.RegisterAssemblyModules(assemblies);
			
			var container = builder.Build();
			config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

			// Register the Autofac middleware FIRST. This also adds
			// Autofac-injected middleware registered with the container.
			app.UseAutofacMiddleware(container);
			app.UseAutofacWebApi(config);

			app.UseWebApi(config);
		}
		private static string GetAppBaseDirectory()
		{
			var domain = AppDomain.CurrentDomain;
			return domain.SetupInformation.PrivateBinPath ?? domain.BaseDirectory;
		}
	}
}