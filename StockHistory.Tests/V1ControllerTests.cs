using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Xunit;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockHistory.Common.Enums;
using StockHistory.Common.Models;
using StockHistory.Tests.Mocks;

namespace StockHistory.Tests
{
	[Collection("StockHistory")]
	public class V1ControllerTests
	{
		private HttpClient client;
		private const string TestStockId = "GOOL";
		private const string WrongClientId = "000";

		[Fact]
		public async void GetStockById()
		{
			var response = await client.SendAsync(GetRequestWithAuthorization(TestStockId, TestStockHistoryDataAccess.ValidClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stockData = JsonConvert.DeserializeObject<StockData>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stockData);
			Assert.Equal(TestStockId, stockData.StockId);
			Assert.Empty(stockData.Stats);
		}

		[Fact]
		public async void GetStockByIdIncludeHigh()
		{
			var response = await client.SendAsync(GetRequestWithAuthorization(TestStockId+ "?IncludePriceType=High", TestStockHistoryDataAccess.ValidClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stockData = JsonConvert.DeserializeObject<StockData>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stockData);
			Assert.Equal(TestStockId, stockData.StockId);
			Assert.NotEmpty(stockData.Stats);
			Assert.NotEmpty(stockData.Stats);
			Assert.Equal(1, stockData.Stats.Count);
			Assert.Equal(PriceType.High, stockData.Stats[0].StatName);
			Assert.Equal(TestStockHistoryDataAccess.HighMin, stockData.Stats[0].Min);
			Assert.Equal(TestStockHistoryDataAccess.HighMax, stockData.Stats[0].Max);
		}

		[Fact]
		public async void GetStockByIdIncludeHighAndOpen()
		{
			var response = await client.SendAsync(GetRequestWithAuthorization(TestStockId + "?IncludePriceType=High&IncludePriceType=Open", TestStockHistoryDataAccess.ValidClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stockData = JsonConvert.DeserializeObject<StockData>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stockData);
			Assert.Equal(TestStockId, stockData.StockId);
			Assert.NotEmpty(stockData.Stats);
			Assert.NotEmpty(stockData.Stats);
			Assert.Equal(2, stockData.Stats.Count);
			//response order not guaranteed
			Assert.Equal(PriceType.High, stockData.Stats[0].StatName);
			Assert.Equal(TestStockHistoryDataAccess.HighMin, stockData.Stats[0].Min);
			Assert.Equal(TestStockHistoryDataAccess.HighMax, stockData.Stats[0].Max);
			Assert.Equal(PriceType.Open, stockData.Stats[1].StatName);
			Assert.Equal(TestStockHistoryDataAccess.OpenMin, stockData.Stats[1].Min);
			Assert.Equal(TestStockHistoryDataAccess.OpenMax, stockData.Stats[1].Max);
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
			
			var response = await client.SendAsync(GetRequestWithAuthorization("", TestStockHistoryDataAccess.ValidClientId));
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
				BaseAddress = new Uri($"{CloudConfigurationManager.GetSetting("StockHistory.Uri")}api/v1/stocks/"),
				};
		}
	}
}
