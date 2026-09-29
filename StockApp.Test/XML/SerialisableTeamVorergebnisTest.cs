using NUnit.Framework;
using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb;
using StockApp.XML;
using System;
using System.IO;
using System.Linq;

namespace StockApp.Test.XML;

public class SerialisableTeamVorergebnisTest
{
    [Test]
    public void TestVorergebnisRoundTrip()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"stockapp_vorergebnis_test_{Guid.NewGuid():N}.xml");
        try
        {
            ITurnier turnier = Turnier.Create();
            turnier.SetBewerb(Wettbewerbsart.Team);
            var bewerb = turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
            bewerb.AddNewTeam();

            var team = bewerb.Teams.First();
            team.StrafSpielpunkte = 3;
            team.VorergebnisSpielpunktePlus = 12;
            team.VorergebnisSpielpunkteMinus = 4;
            team.VorergebnisStockpunktePlus = 87;
            team.VorergebnisStockpunkteMinus = 65;

            SavingModule.Save(ref turnier, filePath);

            ITurnier loaded = Turnier.Create();
            LoadingModule.Load(ref loaded, filePath);

            var loadedTeam = loaded.ContainerTeamBewerbe.CurrentTeamBewerb.Teams.First();

            Assert.That(loadedTeam.StrafSpielpunkte, Is.EqualTo(3));
            Assert.That(loadedTeam.VorergebnisSpielpunktePlus, Is.EqualTo(12));
            Assert.That(loadedTeam.VorergebnisSpielpunkteMinus, Is.EqualTo(4));
            Assert.That(loadedTeam.VorergebnisStockpunktePlus, Is.EqualTo(87));
            Assert.That(loadedTeam.VorergebnisStockpunkteMinus, Is.EqualTo(65));
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}

public class SerialisableTeamStockTVNameTest
{
    [Test]
    public void TestStockTVNameRoundTrip()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"stockapp_stocktvname_test_{Guid.NewGuid():N}.xml");
        try
        {
            ITurnier turnier = Turnier.Create();
            turnier.SetBewerb(Wettbewerbsart.Team);
            var bewerb = turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
            bewerb.AddNewTeam();
            bewerb.AddNewTeam();
            var teams = bewerb.Teams.ToList();
            teams[0].TeamName = "Langer Name";
            teams[0].TeamNameStockTV = "Kurz";
            teams[1].TeamName = "Ohne Kurzname";

            SavingModule.Save(ref turnier, filePath);

            ITurnier loaded = Turnier.Create();
            LoadingModule.Load(ref loaded, filePath);

            var loadedTeams = loaded.ContainerTeamBewerbe.CurrentTeamBewerb.Teams.ToList();
            Assert.That(loadedTeams[0].TeamNameStockTV, Is.EqualTo("Kurz"));
            Assert.That(loadedTeams[0].TeamNameForStockTV, Is.EqualTo("Kurz"));
            Assert.That(loadedTeams[1].TeamNameStockTV, Is.Null.Or.Empty);
            Assert.That(loadedTeams[1].TeamNameForStockTV, Is.EqualTo("Ohne Kurzname"));
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
