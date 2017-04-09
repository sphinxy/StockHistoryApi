using System;
using System.Collections.Generic;

namespace StockHistory.Common.Models
{
	public class StockData
	{
		public string StockId { get; set; }

		public DateTime? RecentDataPointTime { get; set; }

		public List<PriceTypeStat> Stats { get; set; }

	}
}
