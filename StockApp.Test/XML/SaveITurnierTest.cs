using NUnit.Framework;
using StockApp.Core.Factories;
using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.XML;
using System;
using System.IO;
using System.Linq;

namespace StockApp.Test.XML;

public class SaveITurnierTest
{
    [Test]
    public void TestSaveMethod_CreatesNonEmptyValidXmlFile()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"stockapp_save_test_{Guid.NewGuid():N}.xml");
        try
        {
            var t = Turnier.Create();
            t.OrgaDaten.EntryFee.Value = 10.0;
            t.OrgaDaten.EntryFee.Verbal = "zehn";
            t.OrgaDaten.Venue = "Hankofen";
            t.OrgaDaten.Operator = "ESF Hankofen";
            t.OrgaDaten.Organizer = "TV Geiselhörung";
            t.OrgaDaten.TournamentName = "Stockturnier der Stockschützen";
            t.OrgaDaten.Referee.Name = "Senft Ludwig";
            t.OrgaDaten.Referee.ClubName = "Kreis 105";
            t.OrgaDaten.CompetitionManager.Name = "Schwanitz Hans";
            t.OrgaDaten.CompetitionManager.ClubName = "KO 105";
            t.OrgaDaten.ComputingOfficer.Name = "Rechenschieber Erwin";
            t.OrgaDaten.ComputingOfficer.ClubName = "SV Kasse";
            t.OrgaDaten.DateOfTournament = System.DateTime.Now;

            t.SetBewerb(StockApp.Core.Wettbewerb.Wettbewerbsart.Team);
            var bewerb = t.ContainerTeamBewerbe.CurrentTeamBewerb;
            for (int i = 0; i < 7; i++)
            {
                bewerb.AddNewTeam();
            }

            GamePlanFactory.MatchTeamAndGames(
                GamePlanFactory.LoadAllGameplans()
                                .First(p => p.Teams == 7), bewerb.Teams);

            bewerb.Games.First().Spielstand.SetMasterTeamAValue(3);
            bewerb.Games.First().Spielstand.SetMasterTeamBValue(11);

            SavingModule.Save(ref t, filePath);
            var xml = SavingModule.ConvertToXml(ref t);

            Assert.That(File.Exists(filePath), Is.True, "Save() sollte die Datei anlegen");
            Assert.That(new FileInfo(filePath).Length, Is.GreaterThan(0), "gespeicherte Datei darf nicht leer sein");
            Assert.That(xml, Is.Not.Null.And.Not.Empty);
            Assert.That(xml, Does.Contain("Hankofen"), "ConvertToXml sollte dieselben Daten wie Save() enthalten");
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
