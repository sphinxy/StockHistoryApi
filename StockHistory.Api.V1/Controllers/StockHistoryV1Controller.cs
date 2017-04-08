using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using StockHistory.Common.Models;
using StockHistory.Logic;

namespace StockHistory.Api.V1.Controllers
{
	[RoutePrefix("api/v1/stocks")]
	public class StockHistoryV1Controller: ApiController
	{
		private readonly IStockHistoryLogic _stockHistoryLogic;
		[HttpGet]
		[Route("")]
		public async Task<List<Stock>> ListStocks()
		{
			var stocks = await _stockHistoryLogic.GetStocks();
			return stocks;
		}

		[HttpGet]
		[Route("{stockId}")]
		public async Task<Stock> GetStock(string stockId)
		{
			var stock = await _stockHistoryLogic.GetStockById(stockId);
			return stock;
		}
		public StockHistoryV1Controller(IStockHistoryLogic stockHistoryLogic)
		{
			if (stockHistoryLogic == null)
			{
				throw new ArgumentNullException();
			}
			_stockHistoryLogic = stockHistoryLogic;
		}
	}
}
