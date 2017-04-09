using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using StockHistory.Common.Enums;
using StockHistory.Common.Models;
using StockHistory.Logic;

namespace StockHistory.Api.V1.Controllers
{
	/// <summary>
	/// Enable uploading stock market data in the CSV format and query some stats after. 
	/// All data is visible only to the owner based on ApiKey
	/// </summary>
	[RoutePrefix("api/v1/stocks")]
	public class StockHistoryV1Controller: ApiController
	{
		private readonly IStockHistoryLogic _stockHistoryLogic;
		private const string SecurityScheme = "ApiKey";
		/// <summary>
		/// List available stocks with the most recent update timestamp.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// query individual stock symbol for the following stats: min, avg, max, median, 95th percentile for each price type (OHLC)
		/// </summary>
		/// <param name="stockId">Stock Id</param>
		/// <param name="includePriceType">One or more stats to include</param>
		/// <returns></returns>
		[HttpGet]
		[Route("{stockId}", Name = "GetStockInfo")]
		public async Task<IHttpActionResult> GetStockInfo(string stockId, [FromUri] List<PriceType?> includePriceType = null)
		{
			var clientId = GetAuthorizationHeader();
			if (clientId == null)
			{
				return Unauthorized(new AuthenticationHeaderValue(SecurityScheme));
			}
			try
			{
				var stockInfo = await _stockHistoryLogic.GetStockInfoById(stockId, clientId, includePriceType?.Where(p => p.HasValue).Select(p => p.Value).ToList());
				if (stockInfo == null)
				{
					return NotFound();
				}
				return Ok(stockInfo);
			}
			catch (Exception)
			{
				return InternalServerError(new Exception("Error while processing request"));
			}
		}

		/// <summary>
		/// Uploading stock market data in the CSV format (as exported by google finance, Date, Open, High, Low, Close, Volume) 
		/// </summary>
		/// <param name="stockId"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("{stockId}")]
		public async Task<IHttpActionResult> Upload(string stockId)
		{
			var clientId = GetAuthorizationHeader();
			if (clientId == null)
			{
				return Unauthorized(new AuthenticationHeaderValue(SecurityScheme));
			}

			try
			{
				if (!Request.Content.IsMimeMultipartContent())
					throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

				var provider = new MultipartMemoryStreamProvider();
				await Request.Content.ReadAsMultipartAsync(provider);
				if (provider.Contents.Count() == 1)
				{
					var file = provider.Contents[0];
					var buffer = await file.ReadAsStreamAsync();
					var result = await _stockHistoryLogic.SaveStockData(stockId, clientId, buffer);
					if (result.Success)
					{
						return Created(Url.Link("GetStockInfo", null), result);
					}
					return Ok(result);
				}

				return Ok(new StockUploadResult { Success = false, Error = "More than one file"});
			}
			catch (Exception)
			{
				return InternalServerError(new Exception("Error while upload file"));
			}
		}

		/// <summary>
		/// Stock History information
		/// </summary>
		/// <param name="stockHistoryLogic"></param>
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
		}
	}
}
