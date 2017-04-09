using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models.Responses;
using Microsoft.Azure;
using StockHistory.Common.Enums;
using StockHistory.Common.Models;

namespace StockHistory.DataAccess
{
	public class StockHistoryDataAccess : IStockHistoryDataAccess
	{
		private string dbName = "StockHistoryTest";
		private string stockMeasure = "stock";
		private string fileMeasure = "stockFile";

		private readonly InfluxDbClient influxDbClient;

		public async Task<StockData> GetStockDataById(string stockId, string clientId, List<PriceType> includePriceTypes)
		{

			var recentDataPointTime = await GetMostRecentDataPoint(stockId, clientId);
			if (recentDataPointTime != null)
			{
				var priceTypeAggrerationsQuery = BuildPriceTypeAggrerations(includePriceTypes);

				//we need at least one field in query, count(volume) for it
				var query =
					$"SELECT COUNT(volume) {priceTypeAggrerationsQuery} FROM {stockMeasure} WHERE clientId='{clientId}' and stockId='{stockId}'";
				var response = await influxDbClient.Client.QueryAsync(dbName, query);
				var stats = response.Count() == 1 ? response.FirstOrDefault() : null;

				var stockData = new StockData
				{
					StockId = stockId,
					RecentDataPointTime = recentDataPointTime,
					Stats = new List<PriceTypeStat>()
				};
				if (stats != null && stats.Values.Count() == 1)
				{
					foreach (var priceType in includePriceTypes)
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
						stockData.Stats.Add(priceTypeStat);
					}
					
				}

				return stockData;
			}

			return null;
		}

		private static double? ParseStatValue(Serie stats, PriceType priceType, StatAggregate statAggregate)
		{
			var priceTypeColumn = priceType.ToString().ToLower();
			var statValue = stats.Values[0][stats.Columns.IndexOf(priceTypeColumn + statAggregate)].ToString().Replace(',', '.');
			double result;
			if (double.TryParse(statValue, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
			{
				return result;
			}
			return null;
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
			var query = $"select * from {stockMeasure} where clientId='{clientId}'  and stockId='{stockId}' order by time desc limit 1";
			var response = await influxDbClient.Client.QueryAsync(dbName, query);
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
			var query = $"select fileId from {fileMeasure} where clientId='{clientId}' group by stockId order by time desc limit 1";
			var response = await influxDbClient.Client.QueryAsync(dbName, query);
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

		public StockHistoryDataAccess()
		{
			influxDbClient = new InfluxDbClient(CloudConfigurationManager.GetSetting("StockHistory.Database.Uri"), CloudConfigurationManager.GetSetting("StockHistory.Database.User"), CloudConfigurationManager.GetSetting("StockHistory.Database.Password"), InfluxDbVersion.Latest);
		}
	}
}
