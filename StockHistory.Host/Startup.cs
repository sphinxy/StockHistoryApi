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
using Swashbuckle.Application;

namespace StockHistory
{
	/// <summary>
	/// OWIN startup file
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// Owin configuration
		/// </summary>
		/// <param name="app"></param>
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
			config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
			config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
			
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
			
			SetupSwagger(config);

			//temporary enable all errors show
			config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

			app.UseWebApi(config);
		}
			
		private static void SetupSwagger(HttpConfiguration config)
		{
			// Enable middleware to serve generated Swagger as a JSON endpoint.
			config.EnableSwagger(c =>
			{
				c.SingleApiVersion("v1", "StockHistory Api V1").Description("Stock history information. Use for example 'ApiKey 123456' as apikey. Any number is valid and represent separate client.");
				c.IncludeXmlComments($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\StockHistory.XML");
				c.IncludeXmlComments($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\StockHistory.Api.V1.XML");
				c.DescribeAllEnumsAsStrings();
				c.ApiKey("apiKey").Description("API Key Authentication").Name("Authorization").In("header");
			}).EnableSwaggerUi(c => { c.EnableApiKeySupport("Authorization", "header"); });
		}

		private static string GetAppBaseDirectory()
		{
			var domain = AppDomain.CurrentDomain;
			return domain.SetupInformation.PrivateBinPath ?? domain.BaseDirectory;
		}
	}
}