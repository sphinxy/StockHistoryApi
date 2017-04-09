using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using StockHistory.Models;

namespace StockHistory.Controllers
{
	/// <summary>
	/// Root controller
	/// </summary>
	[RoutePrefix("")]
	public class DefaultController : ApiController
	{

		/// <summary>
		/// Status of API
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("")]
		public ApiStatus ApiStatus()
		{
			return new ApiStatus();
		}

		/// <summary>
		/// Uploader helper
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[Route("Uploader")]
		public HttpResponseMessage UploaderStatus()
		{
			var response = new HttpResponseMessage();
			var htmlSource = File.ReadAllText(System.Web.HttpContext.Current.Request.MapPath("Views/Uploader.html"));
			response.Content = new StringContent(htmlSource);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
			return response;
		}
	}
}
