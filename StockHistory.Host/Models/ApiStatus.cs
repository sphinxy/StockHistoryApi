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
		/// Hint for user
		/// </summary>
		public string Hint { get; }

		/// <summary>
		/// Status information about API
		/// </summary>
		public ApiStatus()
		{
			Name = "StockHistory Api";
			Hint = "Use /swagger and /uploader";
		}
	}
}