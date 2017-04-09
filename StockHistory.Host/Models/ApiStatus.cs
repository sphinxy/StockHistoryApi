using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StockHistory.Models
{
	/// <summary>
	/// Status information about API
	/// </summary>
	public class ApiStatus
	{
		/// <summary>
		/// Api name
		/// </summary>
		public string Name { get;}

		/// <summary>
		/// Just a note to user about default endpoint
		/// </summary>
		public string DefaultEndpoint { get; }

		/// <summary>
		/// Status information about API
		/// </summary>
		public ApiStatus()
		{
			Name = "StockHistory Api";
			DefaultEndpoint = "api/v1/stocks";
		}
	}
}