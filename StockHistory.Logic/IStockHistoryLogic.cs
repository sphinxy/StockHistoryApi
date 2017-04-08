using System.Collections.Generic;
using System.Threading.Tasks;
using StockHistory.Common.Models;

namespace StockHistory.Logic
{
	public interface IStockHistoryLogic
	{
		Task<Stock> GetStockById(string stockId);
		Task<List<Stock>> GetStocks();
	}
}