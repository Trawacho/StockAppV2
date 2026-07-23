using NUnit.Framework;
using StockApp.Comm.NetMqStockTV;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace StockApp.Test.Comm
{
    /// <summary>
    /// Requires a real StockTV device on the LAN (discovery and/or the hardcoded manual IP below).
    /// Excluded from normal `dotnet test` runs; run explicitly by name when hardware is available.
    /// </summary>
    [Category("Manual")]
    internal class StockTvServiceTest
    {
        StockTVService _service = null;
        [SetUp]
        public void Setup()
        {
            _service = new StockTVService();
        }


        [Test]
        [Explicit("Requires a real StockTV device on the LAN to be discovered.")]
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
        [Explicit("Sends real NetMQ settings messages to a discovered LAN device; requires a real StockTV device to be meaningful.")]
        public void AddManualTest()
        {
            _service ??= new StockTVService();

            // IP per mDNS-Discovery ermitteln statt einer fest hinterlegten IP, da diese
            // sich im Testnetz ändern kann.
            _service.Discover();
            int loopCounter = 0;
            while (!_service.StockTVCollection.Any())
            {
                Thread.Sleep(250);
                loopCounter++;
                if (loopCounter > 10) break;
            }
            Assert.That(_service.StockTVCollection.Any(), Is.True, "kein StockTV-Gerät per mDNS gefunden");

            var discovered = _service.StockTVCollection.First();
            _service.AddManual(discovered.HostName, discovered.IPAddress);

            var manual = _service.StockTVCollection.First(t => t.IPAddress == discovered.IPAddress);
            Assert.That(manual, !Is.Null);

            // Ausgangszustand abfragen (nicht einfach annehmen), damit er am Ende wiederhergestellt werden kann
            Assert.That(WaitForSettingsResponse(manual), Is.True, "keine Antwort auf die initiale GetSettings-Anfrage erhalten");
            var originalColorModus = manual.TVSettings.ColorModus;

            try
            {
                foreach (var target in new[] { ColorMode.Dark, ColorMode.Normal, ColorMode.Dark })
                {
                    SetAndVerifyColorModus(manual, target);
                }
            }
            finally
            {
                // Ursprünglichen Zustand wiederherstellen und ebenfalls verifizieren
                SetAndVerifyColorModus(manual, originalColorModus);
            }

            _service?.Dispose();
        }

        /// <summary>
        /// Sendet den gewünschten ColorModus an StockTV und fragt die Einstellungen anschließend per
        /// <see cref="IStockTV.TVSettingsGet"/> erneut ab, um zu verifizieren, dass der Wert tatsächlich
        /// angekommen ist - statt nur blind zu senden und ein Sleep abzuwarten.
        /// Das Gerät übernimmt eine gesendete Einstellung nicht sofort (Read-after-Write-Verzögerung in der
        /// Firmware); deshalb wird die Abfrage mit Timeout wiederholt, statt nur einmal zu prüfen.
        /// </summary>
        private static void SetAndVerifyColorModus(IStockTV tv, ColorMode target, int timeoutMs = 5000)
        {
            tv.TVSettings.ColorModus = target;
            tv.TVSettingsSend();

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < timeoutMs)
            {
                var remaining = (int)Math.Max(200, timeoutMs - sw.ElapsedMilliseconds);
                if (WaitForSettingsResponse(tv, remaining) && tv.TVSettings.ColorModus == target)
                    return;
            }

            Assert.Fail($"StockTV hat ColorModus={target} nicht innerhalb von {timeoutMs}ms übernommen (zuletzt gelesen: {tv.TVSettings.ColorModus})");
        }

        /// <summary>
        /// Fordert die aktuellen Einstellungen per <see cref="IStockTV.TVSettingsGet"/> an und wartet auf die
        /// vollständige Antwort. <see cref="IStockTVSettings.MessageVersion"/> wird als letztes Feld in
        /// <see cref="IStockTVSettings.SetSettings"/> gesetzt - erst wenn dessen PropertyChanged-Event feuert,
        /// sind alle anderen Felder (inkl. ColorModus) garantiert bereits aktualisiert.
        /// </summary>
        private static bool WaitForSettingsResponse(IStockTV tv, int timeoutMs = 3000)
        {
            using var settingsFullyUpdated = new ManualResetEventSlim(false);

            void Handler(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(IStockTVSettings.MessageVersion))
                    settingsFullyUpdated.Set();
            }

            tv.StockTVSettingsChanged += Handler;
            try
            {
                tv.TVSettingsGet();
                return settingsFullyUpdated.Wait(timeoutMs);
            }
            finally
            {
                tv.StockTVSettingsChanged -= Handler;
            }
        }
    }
}
