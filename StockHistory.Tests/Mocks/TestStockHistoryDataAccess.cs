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
		public const string TestStockId = "GOOL";
		public const int HighMin = 10;
		public const int HighMax = 1000;
		public const int OpenMin = 0;
		public const int OpenMax = 1000;

		public async Task<StockInfo> GetStockInfoById(string stockId, string clientId, List<PriceType> includePriceType)
		{
			if (clientId != ValidClientId)
			{
				return null;
			}
			var stockInfo = new StockInfo
			{
				StockId = stockId,
				RecentDataPointTime = DateTime.UtcNow,
				Stats = new List<PriceTypeStat>()
			};
			if (includePriceType.Contains(PriceType.High))
			{
				var statData = new PriceTypeStat() { StatName = PriceType.High, Min = HighMin, Mean = 500, Max = HighMax, Median = 450, Percentile95 = 950 };
				stockInfo.Stats.Add(statData);
			}
			if (includePriceType.Contains(PriceType.Open))
			{
				var statData = new PriceTypeStat() { StatName = PriceType.Open, Min = OpenMin, Mean = 50, Max = OpenMax, Median = 45, Percentile95 = 95 };
				stockInfo.Stats.Add(statData);
			}
			return stockInfo;
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

		public Task<StockUploadResult> SaveStockData(string stockId, string clientId, List<StockDataPoint> stockDataPoints)
		{
			if (stockDataPoints.Count == 252 
			&& stockDataPoints[0].Date  == DateTime.Parse("31-Mar-17")
			&& stockDataPoints[251].Date == DateTime.Parse("4-Apr-16"))
			{
				return Task.FromResult(new StockUploadResult {Success = true});
			}



			return Task.FromResult(new StockUploadResult { Success = false, Error = "Upload failed"});
		}

		public Task<bool> SaveStockFile(string stockId, string clientId, DateTime fileUploadDate, string fileId)
		{
			if (stockId == TestStockId && clientId == ValidClientId)
			{
				return Task.FromResult(true);
			}
			return Task.FromResult(false);

		}
	}
}
