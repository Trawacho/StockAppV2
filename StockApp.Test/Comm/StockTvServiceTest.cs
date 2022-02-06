using NUnit.Framework;
using StockApp.Comm.NetMqStockTV;
using System.Linq;
using System.Threading;

namespace StockApp.Test.Comm
{
    internal class StockTvServiceTest
    {
        StockTVService _service = null;
        [SetUp]
        public void Setup()
        {
            _service = new StockTVService();
        }


        [Test]
        public void StockTvService()
        {
            _service.Discover();
            int loopCounter = 0;
            while (!_service.StockTVCollection.Any())
            {
                Thread.Sleep(250);
                loopCounter++;
                if (loopCounter > 10) break;
            }

            Assert.IsTrue(_service.StockTVCollection.Any());
            _service.StockTVCollection.First().RemoveFromCollection();

            loopCounter = 0;
            while (_service.StockTVCollection.Any())
            {
                Thread.Sleep(250);
                loopCounter++;
                if (loopCounter > 10) break;
            }

            _service.Dispose();

        }


    }
}
