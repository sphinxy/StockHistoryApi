using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockHistory.Models
{
	public class ApiStatus
	{
		public string Name { get;}

		public string DefaultEndpoint { get; }

		public ApiStatus()
		{
			Name = "StockHistory Api";
			DefaultEndpoint = "api/v1/stocks";
		}
	}
}