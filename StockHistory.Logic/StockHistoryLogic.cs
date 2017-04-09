using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockHistory.Common.Enums;
using StockHistory.Common.Helpers;
using StockHistory.Common.Models;
using StockHistory.DataAccess;

namespace StockHistory.Logic
{
	public class StockHistoryLogic : IStockHistoryLogic
	{
		private IStockHistoryDataAccess _dataAccess;
		public async Task<StockInfo> GetStockInfoById(string stockId, string clientId, List<PriceType> includePriceType)
		{
			if (string.IsNullOrEmpty(stockId))
			{
				throw new ArgumentNullException(stockId);
			}
			if (string.IsNullOrEmpty(clientId))
			{
				throw new ArgumentNullException(clientId);
			}
			return await _dataAccess.GetStockInfoById(stockId, clientId, includePriceType);
		}

		public async Task<List<Stock>> GetStocks(string clientId)
		{
			if (string.IsNullOrEmpty(clientId))
			{
				throw new ArgumentNullException(clientId);
			}
			return await _dataAccess.GetStocks(clientId);
		}

		public async Task<StockUploadResult> SaveStockData(string stockId, string clientId, Stream data)
		{
			var fileUploadDate = DateTime.UtcNow;
			try
			{
				using (var reader = new StreamReader(data))
				{
					if (!CheckFileHeader(reader))
					{
						return new StockUploadResult { Success = false, Error = "Header of csv file is not correct" };
					}
					
					var stockDataPoints = new List<StockDataPoint>();
					var parseDataPointResult = ParseDataPoint(reader, stockDataPoints);
					if (!parseDataPointResult.Item1)
					{
						return new StockUploadResult { Success = false, Error = $"Line {parseDataPointResult.Item2} incorrect data" };
					}
					
					//save data, only then save file info
					var stockDataSaveResult = await _dataAccess.SaveStockData(stockId, clientId, stockDataPoints);
					
					if (stockDataSaveResult.Success)
					{
						var stockFileSaveResult = await _dataAccess.SaveStockFile(stockId, clientId, fileUploadDate, new Guid().ToString());
						if (stockFileSaveResult)
						{
							return stockDataSaveResult;
						}
							return new StockUploadResult { Success = false, Error = $"Error while saving file meta" };
					}
					return stockDataSaveResult;

				}
			}
			catch (Exception)
			{
				return new StockUploadResult { Success = false, Error = $"File parsing failed" };

			}
		}

		private Tuple<bool, string> ParseDataPoint(StreamReader reader, List<StockDataPoint> stockDataPoints)
		{
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				if (line != null)
				{
					var values = line.Split(',');
					if (values.Length != 6)
					{
						return new Tuple<bool, string>(false, line);
					}
					var stockDataPoint = new StockDataPoint
					{
						Date = DateTime.Parse(values[0]),
						Open = ParseHelper.ParseDouble(values[1]),
						High = ParseHelper.ParseDouble(values[2]),
						Low = ParseHelper.ParseDouble(values[3]),
						Close = ParseHelper.ParseDouble(values[4]),
						Volume = ParseHelper.ParseDouble(values[5])
					};
					stockDataPoints.Add(stockDataPoint);
				}
			}
			return new Tuple<bool, string>(true, null);
		}

		private bool CheckFileHeader(StreamReader reader)
		{
			if (!reader.EndOfStream)
			{
				var headerLine = reader.ReadLine();
				if (headerLine != "Date,Open,High,Low,Close,Volume")
				{
					return false;
				}
			}
			return true;
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
