using NUnit.Framework;
using StockApp.Core.Factories;
using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.XML;
using System;
using System.IO;
using System.Linq;

namespace StockApp.Test.XML;

public class LoadITurnierTest
{
    private static string NewTempFilePath() =>
        Path.Combine(Path.GetTempPath(), $"stockapp_load_test_{Guid.NewGuid():N}.xml");

    [Test]
    public void TestLoadMethod_OrgaDaten_RoundTripsCorrectly()
    {
        var filePath = NewTempFilePath();
        try
        {
            var turnier = Turnier.Create();
            turnier.OrgaDaten.Venue = "Hankofen";
            turnier.OrgaDaten.Operator = "ESF Hankofen";
            turnier.OrgaDaten.Organizer = "TV Geiselhörung";
            turnier.OrgaDaten.TournamentName = "Stockturnier der Stockschützen";
            turnier.OrgaDaten.EntryFee.Value = 10.0;
            turnier.OrgaDaten.EntryFee.Verbal = "zehn";
            turnier.OrgaDaten.Referee.Name = "Senft Ludwig";
            turnier.OrgaDaten.Referee.ClubName = "Kreis 105";
            // Nur das Datum (ohne Uhrzeit) wird persistiert - siehe SerialisableOrganisation.
            turnier.OrgaDaten.DateOfTournament = new DateTime(2026, 3, 14, 18, 30, 0);

            SavingModule.Save(ref turnier, filePath);

            ITurnier loaded = Turnier.Create();
            LoadingModule.Load(ref loaded, filePath);

            Assert.That(loaded.OrgaDaten.Venue, Is.EqualTo("Hankofen"));
            Assert.That(loaded.OrgaDaten.Operator, Is.EqualTo("ESF Hankofen"));
            Assert.That(loaded.OrgaDaten.Organizer, Is.EqualTo("TV Geiselhörung"));
            Assert.That(loaded.OrgaDaten.TournamentName, Is.EqualTo("Stockturnier der Stockschützen"));
            Assert.That(loaded.OrgaDaten.EntryFee.Value, Is.EqualTo(10.0));
            Assert.That(loaded.OrgaDaten.EntryFee.Verbal, Is.EqualTo("zehn"));
            Assert.That(loaded.OrgaDaten.Referee.Name, Is.EqualTo("Senft Ludwig"));
            Assert.That(loaded.OrgaDaten.Referee.ClubName, Is.EqualTo("Kreis 105"));
            Assert.That(loaded.OrgaDaten.DateOfTournament, Is.EqualTo(new DateTime(2026, 3, 14)),
                "nur das Datum, nicht die Uhrzeit, wird gespeichert");

            // static Load(path) Überladung liefert den rohen XML-Inhalt der Datei
            var rawXml = LoadingModule.Load(filePath);
            Assert.That(rawXml, Does.Contain("Hankofen"));
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Test]
    public void TestLoadMethod_TeamBewerb_RoundTripsTeamsAndGameResults()
    {
        var filePath = NewTempFilePath();
        try
        {
            var turnier = Turnier.Create();
            turnier.SetBewerb(Wettbewerbsart.Team);
            var bewerb = turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
            for (int i = 0; i < 7; i++)
                bewerb.AddNewTeam();

            GamePlanFactory.MatchTeamAndGames(
                GamePlanFactory.LoadAllGameplans().First(p => p.Teams == 7), bewerb.Teams);

            var firstTeam = bewerb.Teams.First();
            firstTeam.TeamName = "SV Testhausen";

            var scoredGame = bewerb.Games.First();
            scoredGame.Spielstand.SetMasterTeamAValue(3);
            scoredGame.Spielstand.SetMasterTeamBValue(11);

            SavingModule.Save(ref turnier, filePath);

            ITurnier loaded = Turnier.Create();
            LoadingModule.Load(ref loaded, filePath);

            var loadedBewerb = loaded.ContainerTeamBewerbe.CurrentTeamBewerb;
            Assert.That(loadedBewerb.Teams.Count(), Is.EqualTo(7));
            Assert.That(loadedBewerb.Teams.First(t => t.StartNumber == firstTeam.StartNumber).TeamName, Is.EqualTo("SV Testhausen"));

            var loadedScoredGame = loadedBewerb.Games.First(g =>
                g.GameNumberOverAll == scoredGame.GameNumberOverAll && g.CourtNumber == scoredGame.CourtNumber);
            Assert.That(loadedScoredGame.Spielstand.GetStockPunkteTeamA(live: false), Is.EqualTo(3));
            Assert.That(loadedScoredGame.Spielstand.GetStockPunkteTeamB(live: false), Is.EqualTo(11));
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Test]
    public void TestLoadMethod_ZielBewerb_RoundTripsTeilnehmerAndResults()
    {
        var filePath = NewTempFilePath();
        try
        {
            var turnier = Turnier.Create();
            turnier.SetBewerb(Wettbewerbsart.Ziel);
            var zielBewerb = turnier.Wettbewerb as IZielBewerb;
            // ZielBewerb.Create() legt bereits einen ersten Teilnehmer an - hier wird bewusst ein
            // ZWEITER hinzugefügt, um auch das Roundtripping einer Mehrfach-Liste abzudecken.
            zielBewerb.AddTeilnehmer(Teilnehmer.Create());

            var teilnehmer = zielBewerb.Teilnehmerliste.Last();
            teilnehmer.FirstName = "Erika";
            teilnehmer.LastName = "Musterfrau";
            teilnehmer.Wertungen.First().Disziplinen.First().Versuch1 = 4;
            teilnehmer.Wertungen.First().Disziplinen.Last().Versuch6 = 10;

            SavingModule.Save(ref turnier, filePath);

            ITurnier loaded = Turnier.Create();
            LoadingModule.Load(ref loaded, filePath);

            var loadedZielBewerb = loaded.Wettbewerb as IZielBewerb;
            Assert.That(loadedZielBewerb, Is.Not.Null, "aktiver Wettbewerb muss nach dem Laden wieder Ziel sein");
            Assert.That(loadedZielBewerb.Teilnehmerliste.Count(), Is.EqualTo(2));

            var loadedTeilnehmer = loadedZielBewerb.Teilnehmerliste.First(t => t.Startnummer == teilnehmer.Startnummer);
            Assert.That(loadedTeilnehmer.FirstName, Is.EqualTo("Erika"));
            Assert.That(loadedTeilnehmer.LastName, Is.EqualTo("Musterfrau"));
            Assert.That(loadedTeilnehmer.Wertungen.First().Disziplinen.First().Versuch1, Is.EqualTo(4));
            Assert.That(loadedTeilnehmer.Wertungen.First().Disziplinen.Last().Versuch6, Is.EqualTo(10));
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
