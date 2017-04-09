using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using StockHistory.Common.Enums;
using StockHistory.Common.Models;

namespace StockHistory.Logic
{
	public interface IStockHistoryLogic
	{
		Task<StockInfo> GetStockInfoById(string stockId, string clientId, List<PriceType> includePriceType);
		Task<List<Stock>> GetStocks(string clientId);

		Task<StockUploadResult> SaveStockData(string stockId, string clientId, Stream data);
	}
}