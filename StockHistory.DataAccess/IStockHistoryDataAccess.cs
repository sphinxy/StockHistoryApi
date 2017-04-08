using System.Collections.Generic;
using System.Threading.Tasks;
using StockHistory.Common.Models;

namespace StockHistory.DataAccess
{
	public interface IStockHistoryDataAccess
	{
		Task<Stock> GetStockById(string stockId);
		Task<List<Stock>> GetStocks();
	}
}