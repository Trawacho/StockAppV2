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

            Assert.That(_service.StockTVCollection.Any(), Is.True);
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

        [Test]
        public void AddManualTest()
        {
            _service ??= new StockTVService();
            _service.AddManual("hostname", "192.168.100.136");
            var manual = _service.StockTVCollection.First();
            Assert.That(manual, !Is.Null);
            manual.TVSettings.ColorModus = ColorMode.Dark;
            manual.TVSettingsSend();

            Thread.Sleep(500);
            manual.TVSettings.ColorModus = ColorMode.Normal;
            manual.TVSettingsSend();

            Thread.Sleep(500);
            manual.TVSettings.ColorModus = ColorMode.Dark;
            manual.TVSettingsSend();

            _service?.Dispose();
        }
    }
}
