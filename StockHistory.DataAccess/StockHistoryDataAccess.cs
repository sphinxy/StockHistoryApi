using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHistory.Common.Models;

namespace StockHistory.DataAccess
{
	public class StockHistoryDataAccess : IStockHistoryDataAccess
	{
		private string validClientId = "123";
		public async Task<Stock> GetStockById(string stockId, string clientId)
		{
			if (clientId != validClientId)
			{
				return null;
			}
			var stock = new Stock { StockId = stockId, LastUpdate = DateTime.UtcNow };
			return stock;
		}

		public async Task<List<Stock>> GetStocks(string clientId)
		{
			if (clientId != validClientId)
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
