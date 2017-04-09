using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
		private string TestStockId;
		private string ValidClientId;
		private const string WrongClientId = "000";

		[Fact]
		public async void GetStockById()
		{
			var response = await client.SendAsync(GetRequestWithAuthorization(TestStockId, ValidClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stockInfo = JsonConvert.DeserializeObject<StockInfo>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stockInfo);
			Assert.Equal(TestStockId, stockInfo.StockId);
			Assert.Empty(stockInfo.Stats);
		}

		[Fact]
		public async void GetStockByIdIncludeHigh()
		{
			var response = await client.SendAsync(GetRequestWithAuthorization(TestStockId+ "?IncludePriceType=High", ValidClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stockInfo = JsonConvert.DeserializeObject<StockInfo>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stockInfo);
			Assert.Equal(TestStockId, stockInfo.StockId);
			Assert.NotEmpty(stockInfo.Stats);
			Assert.NotEmpty(stockInfo.Stats);
			Assert.Equal(1, stockInfo.Stats.Count);
			Assert.Equal(PriceType.High, stockInfo.Stats[0].StatName);
			Assert.Equal(TestStockHistoryDataAccess.HighMin, stockInfo.Stats[0].Min);
			Assert.Equal(TestStockHistoryDataAccess.HighMax, stockInfo.Stats[0].Max);
		}

		[Fact]
		public async void GetStockByIdIncludeHighAndOpen()
		{
			var response = await client.SendAsync(GetRequestWithAuthorization(TestStockId + "?IncludePriceType=High&IncludePriceType=Open", ValidClientId));
			Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}");
			var stockInfo = JsonConvert.DeserializeObject<StockInfo>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(stockInfo);
			Assert.Equal(TestStockId, stockInfo.StockId);
			Assert.NotEmpty(stockInfo.Stats);
			Assert.NotEmpty(stockInfo.Stats);
			Assert.Equal(2, stockInfo.Stats.Count);
			//response order not guaranteed
			Assert.Equal(PriceType.High, stockInfo.Stats[0].StatName);
			Assert.Equal(TestStockHistoryDataAccess.HighMin, stockInfo.Stats[0].Min);
			Assert.Equal(TestStockHistoryDataAccess.HighMax, stockInfo.Stats[0].Max);
			Assert.Equal(PriceType.Open, stockInfo.Stats[1].StatName);
			Assert.Equal(TestStockHistoryDataAccess.OpenMin, stockInfo.Stats[1].Min);
			Assert.Equal(TestStockHistoryDataAccess.OpenMax, stockInfo.Stats[1].Max);
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

		[Fact]
		public async void PostStocks()
		{
			var response = await PostStockFile("TestData/googCorrect252.csv"); 
			Assert.Equal(HttpStatusCode.Created, response.StatusCode);
			var uploadResult = JsonConvert.DeserializeObject<StockUploadResult>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(uploadResult);
			Assert.True(uploadResult.Success);
		}

		[Fact]
		public async void PostStocksWrongHeader()
		{
			var response = await PostStockFile("TestData/googWrongHeader.csv");
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var uploadResult = JsonConvert.DeserializeObject<StockUploadResult>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(uploadResult);
			Assert.False(uploadResult.Success);
			Assert.True(uploadResult.Error.StartsWith("Header"));
		}

		[Fact]
		public async void PostStocksWrongLine()
		{
			var response = await PostStockFile("TestData/googWrongLine.csv");
			Assert.Equal(HttpStatusCode.OK, response.StatusCode);
			var uploadResult = JsonConvert.DeserializeObject<StockUploadResult>(await response.Content.ReadAsStringAsync());
			Assert.NotNull(uploadResult);
			Assert.False(uploadResult.Success);
			Assert.True(uploadResult.Error.StartsWith("Line"));
		}

		private async Task<HttpResponseMessage> PostStockFile(string fileName)
		{
			var googTestFile = File.Open(fileName, FileMode.Open);
			HttpContent fileStreamContent = new StreamContent(googTestFile);
			var formData = new MultipartFormDataContent();

			formData.Add(fileStreamContent);
			var response =
				await client.SendAsync(PostRequestWithAuthorization(TestStockId, ValidClientId, formData));
			return response;
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

		private HttpRequestMessage PostRequestWithAuthorization(string uri, string clientId, HttpContent content)
		{
			var request = new HttpRequestMessage()
			{
				RequestUri = new Uri(client.BaseAddress + uri),
				Method = HttpMethod.Post, 
				Content = content
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
			TestStockId = TestStockHistoryDataAccess.TestStockId;
			ValidClientId = TestStockHistoryDataAccess.ValidClientId;
		}
	}
}
