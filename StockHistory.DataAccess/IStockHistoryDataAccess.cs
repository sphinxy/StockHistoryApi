using System.Collections.Generic;
using System.Threading.Tasks;
using StockHistory.Common.Models;
using StockHistory.Common.Enums;

namespace StockHistory.DataAccess
{
	public interface IStockHistoryDataAccess
	{
		Task<StockData> GetStockDataById(string stockId, string clientId, List<PriceType> includePriceTypes);
		Task<List<Stock>> GetStocks(string clientId);
	}
}