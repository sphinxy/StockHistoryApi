using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using System.Configuration;

namespace StockHistory.App
{
	class Program
	{
		static void Main(string[] args)
		{
			string baseAddress = ConfigurationManager.AppSettings["StockHistory.Uri"];

			// Start OWIN host 
			using (WebApp.Start<Startup>(url: baseAddress))
			{
				Console.WriteLine("Service started");
				Console.ReadLine();
			}
		}
	}
}
