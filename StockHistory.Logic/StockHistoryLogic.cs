using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHistory.Common.Enums;
using StockHistory.Common.Models;
using StockHistory.DataAccess;

namespace StockHistory.Logic
{
	public class StockHistoryLogic : IStockHistoryLogic
	{
		private IStockHistoryDataAccess _dataAccess;
		public async Task<StockData> GetStockDataById(string stockId, string clientId, List<PriceType> includePriceTypes)
		{
			if (string.IsNullOrEmpty(stockId))
			{
				throw new ArgumentNullException(stockId);
			}
			if (string.IsNullOrEmpty(clientId))
			{
				throw new ArgumentNullException(clientId);
			}
			return await _dataAccess.GetStockDataById(stockId, clientId, includePriceTypes);
		}

		public async Task<List<Stock>> GetStocks(string clientId)
		{
			if (string.IsNullOrEmpty(clientId))
			{
				throw new ArgumentNullException(clientId);
			}
			return await _dataAccess.GetStocks(clientId);
		}
	

		public StockHistoryLogic(IStockHistoryDataAccess dataAccess)
		{
			if (dataAccess == null)
			{
				throw new ArgumentNullException();
			}
			_dataAccess = dataAccess;
		}
	}
}
