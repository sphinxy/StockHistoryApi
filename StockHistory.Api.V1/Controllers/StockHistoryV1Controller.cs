using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using StockHistory.Common.Enums;
using StockHistory.Common.Models;
using StockHistory.Logic;

namespace StockHistory.Api.V1.Controllers
{
	[RoutePrefix("api/v1/stocks")]
	public class StockHistoryV1Controller: ApiController
	{
		private readonly IStockHistoryLogic _stockHistoryLogic;
		private const string SecurityScheme = "ApiKey";
		[HttpGet]
		[Route("")]
		public async Task<IHttpActionResult> ListStocks()
		{
			var clientId = GetAuthorizationHeader();
			if (clientId == null)
			{
				return Unauthorized(new AuthenticationHeaderValue(SecurityScheme));
			}
			try
			{
				var stocks = await _stockHistoryLogic.GetStocks(clientId);
				return Ok(stocks);
			}
			catch (Exception)
			{
				return InternalServerError(new Exception("Error while processing request"));
			}
		}

		[HttpGet]
		[Route("{stockId}")]
		public async Task<IHttpActionResult> GetStockData(string stockId, [FromUri] List<PriceType?> includePriceType)
		{
			var clientId = GetAuthorizationHeader();
			if (clientId == null)
			{
				return Unauthorized(new AuthenticationHeaderValue(SecurityScheme));
			}
			try
			{
				var stockData = await _stockHistoryLogic.GetStockDataById(stockId, clientId, includePriceType.Where(p => p.HasValue).Select(p => p.Value).ToList());
				if (stockData == null)
				{
					return NotFound();
				}
				return Ok(stockData);
			}
			catch (Exception)
			{
				return InternalServerError(new Exception("Error while processing request"));
			}
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
			if (headerScheme != null && headerScheme.Equals(SecurityScheme, StringComparison.OrdinalIgnoreCase))
			{
				return headerValue;
			}
			return null;
			//comment line above just for easy development
			//var temporaryClientId = "123";
			//return temporaryClientId;
			
		}
	}
}
