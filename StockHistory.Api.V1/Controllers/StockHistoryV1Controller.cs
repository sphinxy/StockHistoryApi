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
			var clientId = GetAuthorizationHeader();
			var stocks = await _stockHistoryLogic.GetStocks(clientId);
			return stocks;
		}

		[HttpGet]
		[Route("{stockId}")]
		public async Task<IHttpActionResult> GetStock(string stockId)
		{
			var clientId = GetAuthorizationHeader();
			var stock = await _stockHistoryLogic.GetStockById(stockId, clientId);
			if (stock == null)
			{
				return NotFound();
			}
			return Ok(stock);
		}
		public StockHistoryV1Controller(IStockHistoryLogic stockHistoryLogic)
		{
			if (stockHistoryLogic == null)
			{
				throw new ArgumentNullException();
			}
			_stockHistoryLogic = stockHistoryLogic;
		}

		private string GetAuthorizationHeader()
		{
			var headerScheme = Request.Headers?.Authorization?.Scheme;
			var headerValue = Request.Headers?.Authorization?.Parameter;
			if (headerScheme != null && headerScheme.Equals("Apikey", StringComparison.OrdinalIgnoreCase))
			{
				return headerValue;
			}
			return null;
		}
	}
}
