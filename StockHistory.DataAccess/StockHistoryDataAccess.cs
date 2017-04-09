using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using Microsoft.Azure;
using StockHistory.Common.Enums;
using StockHistory.Common.Helpers;
using StockHistory.Common.Models;

namespace StockHistory.DataAccess
{
	public class StockHistoryDataAccess : IStockHistoryDataAccess
	{
		private string dbName = "StockHistoryTest";
		private const string StockMeasure = "stock";
		private const string FileMeasure = "stockFile";
		private const string ClientIdTag = "clientId";
		private const string StockIdTag = "stockId";

		private readonly InfluxDbClient _influxDbClient;

		public async Task<StockInfo> GetStockInfoById(string stockId, string clientId, List<PriceType> includePriceType)
		{
			var recentDataPointTime = await GetMostRecentDataPoint(stockId, clientId);
			if (recentDataPointTime != null)
			{
				var priceTypeAggrerationsQuery = BuildPriceTypeAggrerations(includePriceType);

				//we need at least one field in query, count(volume) for it
				var query =
					$"SELECT COUNT(volume) {priceTypeAggrerationsQuery} FROM {StockMeasure} WHERE {ClientIdTag}='{clientId}' and {StockIdTag}='{stockId}'";
				var response = await _influxDbClient.Client.QueryAsync(dbName, query);
				var stats = response.Count() == 1 ? response.FirstOrDefault() : null;

				var stockInfo = new StockInfo
				{
					StockId = stockId,
					RecentDataPointTime = recentDataPointTime,
					Stats = new List<PriceTypeStat>()
				};
				if (stats != null && stats.Values.Count() == 1)
				{
					foreach (var priceType in includePriceType)
					{

						var priceTypeStat = new PriceTypeStat
						{
							StatName = priceType,
							Min = ParseStatValue(stats, priceType, StatAggregate.Min),
							Mean = ParseStatValue(stats, priceType, StatAggregate.Mean),
							Max = ParseStatValue(stats, priceType, StatAggregate.Max),
							Median = ParseStatValue(stats, priceType, StatAggregate.Median),
							Percentile95 = ParseStatValue(stats, priceType, StatAggregate.Percentile),
						};
						stockInfo.Stats.Add(priceTypeStat);
					}
					
				}

				return stockInfo;
			}

			return null;
		}

		private static double? ParseStatValue(Serie stats, PriceType priceType, StatAggregate statAggregate)
		{
			var priceTypeColumn = priceType.ToString().ToLower();
			var statValue = stats.Values[0][stats.Columns.IndexOf(priceTypeColumn + statAggregate)].ToString().Replace(',', '.');
			return ParseHelper.ParseDouble(statValue);
		}

		private static string BuildPriceTypeAggrerations(List<PriceType> includePriceTypes)
		{
			var aggregates = new []
			{
				StatAggregate.Min+"({0}) as {0}"+StatAggregate.Min,
				StatAggregate.Mean+"({0}) as {0}"+StatAggregate.Mean,
				StatAggregate.Max+"({0}) as {0}"+StatAggregate.Max,
				StatAggregate.Median+"({0}) as {0}"+StatAggregate.Median,
				StatAggregate.Percentile+"({0}, 95) as {0}"+StatAggregate.Percentile
			};
			var sb = new StringBuilder();
			foreach (var priceType in includePriceTypes)
			{
				var priceTypeColumn = priceType.ToString().ToLower();
				foreach (var aggregate in aggregates)
				{
					sb.Append(" , ");
					sb.Append(string.Format(aggregate, priceTypeColumn));
				}
			}
			var priceTypeAggrerationsQuery = sb.ToString();
			return priceTypeAggrerationsQuery;
		}

		public async Task<DateTime?> GetMostRecentDataPoint(string stockId, string clientId)
		{
			var query = $"select * from {StockMeasure} where clientId='{clientId}'  and stockId='{stockId}' order by time desc limit 1";
			var response = await _influxDbClient.Client.QueryAsync(dbName, query);
			foreach (var recentPoint in response)
			{
				if (recentPoint.Values.Count == 1)
				{
					return (DateTime) recentPoint.Values[0][recentPoint.Columns.IndexOf("time")];
				}
			}
			return null;
		}



		public async Task<List<Stock>> GetStocks(string clientId)
		{
			var stocks = new List<Stock>();
			var query = $"select fileId from {FileMeasure} where {ClientIdTag}='{clientId}' group by {StockIdTag} order by time desc limit 1";
			var response = await _influxDbClient.Client.QueryAsync(dbName, query);
			foreach (var stock in response)
			{
				if (stock.Values.Count == 1)
				{
					stocks.Add(new Stock
					{
						StockId = stock.Tags["stockId"],
						LastUpdate = (DateTime) stock.Values[0][stock.Columns.IndexOf("time")]
					});
				}
			}
			//sort manually due InfluxDB can sort only by time
			stocks = stocks.OrderBy(s => s.StockId).ToList();

			return stocks;
		}

		public async Task<StockUploadResult> SaveStockData(string stockId, string clientId, List<StockDataPoint> stockDataPoints)
		{
			var points = new List<Point>();
			try
			{
				foreach (var stockDataPoint in stockDataPoints)
				{
					var pointToWrite = new Point()
					{
						Name = StockMeasure,
						Tags = new Dictionary<string, object>()
						{
							{ClientIdTag, clientId},
							{StockIdTag, stockId}
						},
						Fields = new Dictionary<string, object>()
						{
							{"open", stockDataPoint.Open},
							{"high", stockDataPoint.High},
							{"low", stockDataPoint.Low},
							{"close", stockDataPoint.Close},
							{"volume", stockDataPoint.Volume},

						},
						Timestamp = stockDataPoint.Date
					};
					points.Add(pointToWrite);
				}
				var result = await _influxDbClient.Client.WriteAsync(dbName, points);
				return new StockUploadResult { Success = result.Success };
			}
			catch (Exception)
			{
				return new StockUploadResult {Success = false, Error = $"Saving in db failed"};
			}
		}

		public async Task<bool> SaveStockFile(string stockId, string clientId, DateTime fileUploadDate, string fileId)
		{
			var filePointToWrite = new Point()
			{
				Name = FileMeasure,
				Tags = new Dictionary<string, object>()
						{
							{ClientIdTag, clientId},
							{StockIdTag, stockId}
						},
				Fields = new Dictionary<string, object>()
						{
							{"fileId", fileId}
						},
				Timestamp = fileUploadDate
			};
			var result = await _influxDbClient.Client.WriteAsync(dbName, filePointToWrite);
			return result.Success;
		}

		public StockHistoryDataAccess()
		{
			_influxDbClient = new InfluxDbClient(CloudConfigurationManager.GetSetting("StockHistory.Database.Uri"), CloudConfigurationManager.GetSetting("StockHistory.Database.User"), CloudConfigurationManager.GetSetting("StockHistory.Database.Password"), InfluxDbVersion.Latest);
		}
	}
}
