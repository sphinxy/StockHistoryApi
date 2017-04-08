using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StockHistory.Tests
{
	[CollectionDefinition("StockHistory")]
	public class UsersCollection : ICollectionFixture<SelfHostFixture<Startup>>
	{
		//https://xunit.github.io/docs/shared-context.html#class-fixture
	}
}
