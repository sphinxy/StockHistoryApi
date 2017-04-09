using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Configuration;
using Microsoft.Azure;

namespace StockHistory.App
{
	class Program
	{
		static void Main(string[] args)
		{
			string baseAddress = CloudConfigurationManager.GetSetting("StockHistory.Uri");

			// Start OWIN host 
			using (WebApp.Start<Startup>(url: baseAddress))
			{
				Console.WriteLine("Service started");
				Console.ReadLine();
			}
		}
	}
}
