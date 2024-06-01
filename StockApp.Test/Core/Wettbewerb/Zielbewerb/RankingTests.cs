using NUnit.Framework;
using StockApp.Comm.NetMqStockTV;
using StockApp.Core.Wettbewerb.Zielbewerb;
using System.Linq;

namespace StockApp.Test
{
    public class RankingTests
    {
        IZielBewerb _bewerb;

        [SetUp]
        public void Setup()
        {
            // Bewerb mit 3 Teilnehmer, der erste ist bereits angelegt
            _bewerb = ZielBewerb.Create();
            _bewerb.AddNewTeilnehmer();
            _bewerb.AddNewTeilnehmer();

            for (int tln = 0; tln <= 2; tln++)  //  jeder Teilnehmer
            {
                for (int w = 0; w <= 2; w++)    //  jede Wertung
                {
                    if (_bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.Count() <= w)
                        _bewerb.Teilnehmerliste.ElementAt(tln).AddNewWertung();

                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch1 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch2 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch3 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch4 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch5 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch6 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch1 = 5;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch2 = 5;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch3 = 5;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch4 = 5;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch5 = 5;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Schiessen).Versuch6 = 5;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch1 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch2 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch3 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch4 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch5 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenSeite).Versuch6 = 6;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch1 = 4;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch2 = 4;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch3 = 4;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch4 = 4;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch5 = 4;
                    _bewerb.Teilnehmerliste.ElementAt(tln).Wertungen.ElementAt(w).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.Kombinieren).Versuch6 = 4;


                }
            }
        }



        [Test]
        public void TestRanking()
        {
            //Gesamtpunkte unterschiedlich
            _bewerb.Teilnehmerliste.First(t => t.Startnummer == 1).Wertungen.ElementAt(0).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch1 = 6;
            _bewerb.Teilnehmerliste.First(t => t.Startnummer == 2).Wertungen.ElementAt(1).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch1 = 8;
            _bewerb.Teilnehmerliste.First(t => t.Startnummer == 3).Wertungen.ElementAt(2).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch1 = 4;

            var ranked = _bewerb.GetTeilnehmerRanked();

            Assert.That(ranked.ElementAt(0).Startnummer, Is.EqualTo(2));    //Startnummer 2 ist auf Platz 1
            Assert.That(ranked.ElementAt(1).Startnummer, Is.EqualTo(1));    //Startnummer 1 ist auf Platz 2
            Assert.That(ranked.ElementAt(2).Startnummer, Is.EqualTo(3));    //Startnummer 3 ist auf Platz 3

            //Gesamtpunkte wieder gleich setzen
            _bewerb.Teilnehmerliste.First(t => t.Startnummer == 1).Wertungen.ElementAt(0).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch1 = 6;
            _bewerb.Teilnehmerliste.First(t => t.Startnummer == 2).Wertungen.ElementAt(1).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch1 = 6;
            _bewerb.Teilnehmerliste.First(t => t.Startnummer == 3).Wertungen.ElementAt(2).Disziplinen.First(t => t.Disziplinart == StockTVZielDisziplinName.MassenMitte).Versuch1 = 6;

        }


    }
}