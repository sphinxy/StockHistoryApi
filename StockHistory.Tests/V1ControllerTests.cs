using System;
using System.Collections.Generic;
using System.Configuration;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockHistory.Common.Models;

namespace StockHistory.Tests
{
	[Collection("StockHistory")]
	public class V1ControllerTests
	{
		private HttpClient client;

		[Fact]
		public async void GetStockById()
		{
			var testStockId = "GOOL";
			var response = await client.GetAsync(testStockId);
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stock = JsonConvert.DeserializeObject<Stock>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stock);
			Assert.Equal(testStockId, stock.StockId);
		}
		
		[Fact]
		public async void GetStocks()
		{
			
			var response = await client.GetAsync("");
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stocks = JsonConvert.DeserializeObject<List<Stock>>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stocks);
			Assert.Equal(2, stocks.Count);
		}

		//[Fact]
		//public async void Delay()
		//{

		//	await Task.Delay(10000);
		//}

		public V1ControllerTests()
		{
			client = new HttpClient
			{
				BaseAddress = new Uri($"{ConfigurationManager.AppSettings["StockHistory.Uri"]}api/v1/stocks/")
			};
		}
	}
}
