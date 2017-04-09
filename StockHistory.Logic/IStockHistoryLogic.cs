using System.Collections.Generic;
using System.Threading.Tasks;
using StockHistory.Common.Enums;
using StockHistory.Common.Models;

namespace StockHistory.Logic
{
	public interface IStockHistoryLogic
	{
		Task<StockData> GetStockDataById(string stockId, string clientId, List<PriceType> includePriceTypes);
		Task<List<Stock>> GetStocks(string clientId);
	}
}