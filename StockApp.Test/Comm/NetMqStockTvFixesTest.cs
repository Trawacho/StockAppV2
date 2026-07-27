using NetMQ;
using NUnit.Framework;
using StockApp.Comm.MDns;
using StockApp.Comm.NetMqStockTV;
using System;

namespace StockApp.Test.Comm
{
    /// <summary>
    /// Regression tests for the fixes on branch fix/netmq-stocktv-stability.
    /// Uses manual (non-mDNS) hosts so no real StockTV hardware is required.
    /// </summary>
    internal class NetMqStockTvFixesTest
    {
        private class TestableAppClient : StockTVAppClient
        {
            public TestableAppClient() : base("127.0.0.1", 4747, "unit-test") { }
            public void InvokeRaiseMessageReceived(NetMQFrame topic, NetMQFrame value) =>
                RaiseMessageReceived(topic, value);
        }

        [Test]
        public void StockTVAppClient_RaiseMessageReceived_UnknownTopic_DoesNotThrow()
        {
            // An exception here would fail the test on its own; no Assert needed.
            using var client = new TestableAppClient();
            client.InvokeRaiseMessageReceived(new NetMQFrame("NotARealTopic"), NetMQFrame.Empty);
        }

        [Test]
        public void StockTVSettings_SetSettings_ShortArray_DoesNotThrow()
        {
            var settings = new StockTVSettings();
            settings.SetSettings(new byte[3]);
            Assert.That(settings.Bahn, Is.EqualTo(0));
        }

        [Test]
        public void StockTVResult_SetResult_DuplicateContent_DoesNotRaiseChangedTwice()
        {
            var result = new StockTVResult();
            int changedCount = 0;
            result.ResultChanged += () => changedCount++;

            byte[] arr1 = new byte[10]; // Bahn=0.., MessageVersion (index 8) = 0
            result.SetResult(arr1);
            Assert.That(changedCount, Is.EqualTo(1));

            byte[] arr2 = (byte[])arr1.Clone(); // same content, different array instance
            result.SetResult(arr2);
            Assert.That(changedCount, Is.EqualTo(1), "identical content must not raise ResultChanged again");

            byte[] arr3 = (byte[])arr1.Clone();
            arr3[0] = 5; // different content
            result.SetResult(arr3);
            Assert.That(changedCount, Is.EqualTo(2));
        }

        [Test]
        public void StockTV_Equals_Null_ReturnsFalse()
        {
            var host = MDnsHost.Create("unit-test-host", "192.168.100.250", 4747, 4748, "n.a.");
            using var tv = new StockTV(host);

            Assert.That(tv.Equals(null), Is.False);
        }

        [Test]
        public void StockTV_StockTVId_IsUniquePerInstance()
        {
            var host1 = MDnsHost.Create("unit-test-host-1", "192.168.100.251", 4747, 4748, "n.a.");
            var host2 = MDnsHost.Create("unit-test-host-2", "192.168.100.252", 4747, 4748, "n.a.");
            using var tv1 = new StockTV(host1);
            using var tv2 = new StockTV(host2);

            Assert.That(tv1.StockTVId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(tv2.StockTVId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(tv1.StockTVId, Is.Not.EqualTo(tv2.StockTVId));
        }
    }
}
