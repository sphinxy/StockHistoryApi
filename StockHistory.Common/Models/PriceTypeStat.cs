using StockHistory.Common.Enums;

namespace StockHistory.Common.Models
{
	public class PriceTypeStat
	{
		public PriceType? StatName { get; set; }
		public double? Min { get; set; }
		public double? Mean { get; set; }
		public double? Max { get; set; }
		public double? Median { get; set; }
		public double? Percentile95 { get; set; }
	}
}
