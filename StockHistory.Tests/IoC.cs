using System.Reflection;
using Autofac;
using StockHistory.DataAccess;
using StockHistory.Tests.Mocks;
using Module = Autofac.Module;

namespace StockHistory.Service
{
	public class IoC : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<TestStockHistoryDataAccess>().As<IStockHistoryDataAccess>().SingleInstance();
			base.Load(builder);
		}
	}
}