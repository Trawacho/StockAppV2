using NUnit.Framework;
using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.XML;
using System.Linq;

namespace StockApp.Test.XML;

public class SaveITurnierTest
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void TestSaveMethod()
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
        bewerb.CreateGames();
        bewerb.Games.First().Spielstand.SetMasterTeamAValue(3);
        bewerb.Games.First().Spielstand.SetMasterTeamBValue(11);

        t.SetBewerb(StockApp.Core.Wettbewerb.Wettbewerbsart.Ziel);
        var zielBewerb = t.Wettbewerb as IZielBewerb;
        zielBewerb.AddTeilnehmer(Teilnehmer.Create());
        zielBewerb.Teilnehmerliste.First().Wertungen.First().Disziplinen.First().Versuch1 = 4;
        zielBewerb.Teilnehmerliste.First().Wertungen.First().Disziplinen.Last().Versuch6 = 10;

        SavingModule.Save(ref t, $@"C:\Temp\test.xml");
        _ = SavingModule.ConvertToXml(ref t);
    }
}
