using Autofac;
using StockHistory.DataAccess;

namespace StockHistory.Service
{
	public class IoC : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<StockHistoryDataAccess>().As<IStockHistoryDataAccess>().SingleInstance().PreserveExistingDefaults();
			base.Load(builder);
		}
	}
}