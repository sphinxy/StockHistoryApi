using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

namespace StockHistory.Tests
{
	public class SelfHostFixture<T> : IDisposable
	{
		protected IDisposable WebApiService;

		protected virtual void OnDispose()
		{
			WebApiService.Dispose();
		}

		protected virtual void OnInit()
		{
			WebApiService = WebApp.Start<T>(new StartOptions(ConfigurationManager.AppSettings["StockHistory.Uri"]));
		}

		public virtual void Dispose()
		{
			OnDispose();
		}

		public SelfHostFixture()
		{
			OnInit();
		}
	}

}
