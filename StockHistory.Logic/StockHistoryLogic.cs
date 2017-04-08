﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHistory.Common.Models;
using StockHistory.DataAccess;

namespace StockHistory.Logic
{
	public class StockHistoryLogic : IStockHistoryLogic
	{
		private IStockHistoryDataAccess _dataAccess;
		public async Task<Stock> GetStockById(string stockId)
		{
			return await _dataAccess.GetStockById(stockId);
		}

		public async Task<List<Stock>> GetStocks()
		{
			return await _dataAccess.GetStocks();
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