using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockHistory.Common.Helpers
{
	public class ParseHelper
	{
		public static double? ParseDouble(string statValue)
		{
			double result;
			if (double.TryParse(statValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return null;
		}
	}
}
