using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockHistory.Common.Models;
using StockHistory.Common.Enums;

namespace StockHistory.DataAccess
{
	public interface IStockHistoryDataAccess
	{
		Task<StockInfo> GetStockInfoById(string stockId, string clientId, List<PriceType> includePriceType);
		Task<List<Stock>> GetStocks(string clientId);

		Task<StockUploadResult> SaveStockData(string stockId, string clientId, List<StockDataPoint> stockDataPoints);
		Task<bool> SaveStockFile(string stockId, string clientId, DateTime fileUploadDate, string fileId);
	}
}