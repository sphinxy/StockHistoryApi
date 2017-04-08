using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
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
		private const string TestStockId = "GOOL";
		private const string ValidClientId = "123";
		private const string WrongClientId = "000";

		[Fact]
		public async void GetStockById()
		{
			var response = await client.SendAsync(GetRequestWithAuthorization(TestStockId, ValidClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stock = JsonConvert.DeserializeObject<Stock>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stock);
			Assert.Equal(TestStockId, stock.StockId);
		}

		[Fact]
		public async void GetStockByIdWrongClient()
		{
			var response = await client.SendAsync(GetRequestWithAuthorization(TestStockId, WrongClientId));
			Assert.Equal(response.StatusCode, HttpStatusCode.NotFound);
		}

		[Fact]
		public async void GetStocks()
		{
			
			var response = await client.SendAsync(GetRequestWithAuthorization("", ValidClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stocks = JsonConvert.DeserializeObject<List<Stock>>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stocks);
			Assert.Equal(2, stocks.Count);
		}

		[Fact]
		public async void GetStocksWrongClient()
		{

			var response = await client.SendAsync(GetRequestWithAuthorization("", WrongClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stocks = JsonConvert.DeserializeObject<List<Stock>>(await response.Content.ReadAsStringAsync());
			Assert.Null(stocks);
		}

		//[Fact]
		//public async void Delay()
		//{

		//	await Task.Delay(10000);
		//}

		private HttpRequestMessage GetRequestWithAuthorization(string uri, string clientId)
		{
			var request = new HttpRequestMessage()
			{
				RequestUri = new Uri(client.BaseAddress + uri),
				Method = HttpMethod.Get,
			};
			request.Headers.Authorization = new AuthenticationHeaderValue("Apikey", clientId);
			return request;

		}

		public V1ControllerTests()
		{
			client = new HttpClient
			{
				BaseAddress = new Uri($"{ConfigurationManager.AppSettings["StockHistory.Uri"]}api/v1/stocks/"),
				};
		}
	}
}
