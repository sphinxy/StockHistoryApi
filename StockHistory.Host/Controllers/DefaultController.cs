using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using StockHistory.Models;

namespace StockHistory.Controllers
{
	[RoutePrefix("")]
	public class DefaultController : ApiController
	{

		[HttpGet]
		[Route("")]
		public ApiStatus ApiStatus()
		{
			return new ApiStatus();
		}
	}
}
