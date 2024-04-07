using StockApp.Core.Turnier;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StockApp.Prints.Spielplan;

public class SpielPlanTemplateViewModel : PrintTemplateViewModelBase
{
    private readonly ITurnier _turnier;

    public SpielPlanTemplateViewModel(ITurnier turnier) : base(turnier)
    {
        _turnier = turnier;
        InitBodyElements();
    }

    private void InitBodyElements()
    {
        BodyElements = new List<Grid>();
        var spielPlanGames = new Spielplan(_turnier.ContainerTeamBewerbe.CurrentTeamBewerb.GetAllGames(false)).Spiele;
        var anzahlBahnen = spielPlanGames.Max(b => b.Bahn);

        BodyElements.Add(GetHeaderGrid(anzahlBahnen));
        for (int spielNummer = 1; spielNummer <= spielPlanGames.Max(g => g.SpielNummer); spielNummer++)
        {
            var spiele = spielPlanGames.Where(v => v.SpielNummer == spielNummer); //Die Spiele dieser Spielrunde
            BodyElements.Add(GetSpielRow(spiele, spielNummer, anzahlBahnen));
        }
    }

    private static Grid GetHeaderGrid(int anzahlBahnen)
    {
        Grid grid = GetGridTemplate(anzahlBahnen);
        for (int i = 1; i <= anzahlBahnen; i++)
        {
            var t = SpielplanUIElements.GetTextblock($"Bahn {i}", FontWeights.Bold);
            Grid.SetColumn(t, i);
            grid.Children.Add(t);
        }
        return grid;
    }

    private static Grid GetGridTemplate(int anzahlBahnen)
    {
        var grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };

        for (int i = 0; i <= anzahlBahnen; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = GridLength.Auto,
                SharedSizeGroup = "i"
            });
        }

        return grid;
    }

    /// <summary>
    /// Erzeugt ein Grid mit den Paarungen pro Spiel
    /// </summary>
    /// <param name="spiele">alle Spiele der spielNummer </param>
    /// <param name="spielNummer">laufende Nummer der Spiele</param>
    /// <param name="anzahlBahnen">maximale Anzahl der Bahnen</param>
    /// <returns></returns>
    private static Grid GetSpielRow(IEnumerable<SpielplanGame> spiele, int spielNummer, int anzahlBahnen)
    {
        Grid grid = GetGridTemplate(anzahlBahnen);

        //Überschrift am Zeilenanfang in Spalte 0
        var t = SpielplanUIElements.GetTextblock($"Spiel {spielNummer}", FontWeights.Bold);
        Grid.SetColumn(t, 0);
        grid.Children.Add(t);

        //Für jedes Spiel eine Zelle
        for (int b = 1; b <= anzahlBahnen; b++)  //von Bahn 1 bis zur letzten Bahn die genutzt wird
        {
            var game = spiele.FirstOrDefault(x => x.Bahn == b);
            if (game == null) continue;

            var te = SpielplanUIElements.GetSpielplanSpielText(game);
            Grid.SetColumn(te, b);
            grid.Children.Add(te);
        }

        return grid;
    }

    public List<Grid> BodyElements { get; set; }

    public string HeaderString => "Spielplan";

}
