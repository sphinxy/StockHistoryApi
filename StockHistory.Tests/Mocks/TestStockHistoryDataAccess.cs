using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockHistory.Common.Enums;
using StockHistory.Common.Models;
using StockHistory.DataAccess;

namespace StockHistory.Tests.Mocks
{
	public class TestStockHistoryDataAccess : IStockHistoryDataAccess
	{
		public const string ValidClientId = "123";
		public const int HighMin = 10;
		public const int HighMax = 1000;
		public const int OpenMin = 0;
		public const int OpenMax = 1000;

		public async Task<StockData> GetStockDataById(string stockId, string clientId, List<PriceType> includePriceTypes)
		{
			if (clientId != ValidClientId)
			{
				return null;
			}
			var stockData = new StockData
			{
				StockId = stockId,
				RecentDataPointTime = DateTime.UtcNow,
				Stats = new List<PriceTypeStat>()
			};
			if (includePriceTypes.Contains(PriceType.High))
			{
				var statData = new PriceTypeStat() { StatName = PriceType.High, Min = HighMin, Mean = 500, Max = HighMax, Median = 450, Percentile95 = 950 };
				stockData.Stats.Add(statData);
			}
			if (includePriceTypes.Contains(PriceType.Open))
			{
				var statData = new PriceTypeStat() { StatName = PriceType.Open, Min = OpenMin, Mean = 50, Max = OpenMax, Median = 45, Percentile95 = 95 };
				stockData.Stats.Add(statData);
			}
			return stockData;
		}

		public async Task<List<Stock>> GetStocks(string clientId)
		{
			if (clientId != ValidClientId)
			{
				return null;
			}
			var stocks = new List<Stock>();
			stocks.Add(new Stock { StockId = "stockOne", LastUpdate = DateTime.UtcNow });
			stocks.Add(new Stock { StockId = "stockTwo", LastUpdate = DateTime.UtcNow });
			return stocks;
		}

	}
}
