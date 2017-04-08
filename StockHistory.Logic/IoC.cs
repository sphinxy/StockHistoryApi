using Autofac;
using StockHistory.Logic;

namespace StockHistory.Service
{
	public class IoC : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<StockHistoryLogic>().As<IStockHistoryLogic>().SingleInstance().PreserveExistingDefaults();
			base.Load(builder);
		}
	}
}