using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Core.Wettbewerb.Zielbewerb;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Components;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace StockApp.Prints.Receipts;

public class ReceiptTemplateViewModel : ViewModelBase
{
    private readonly ITurnier _turnier;

    public ReceiptTemplateViewModel(ITurnier turnier) 
    {
        _turnier = turnier;
        InitBodyElements();
    }

    private void InitBodyElements()
    {
        BodyElements = new();
        if(_turnier.Wettbewerb is IZielBewerb bewerb)
        {
            foreach (var teilnehmer in bewerb.Teilnehmerliste)
            {
                string von = !string.IsNullOrWhiteSpace(teilnehmer.Vereinsname) ? $"{teilnehmer.Name} ({teilnehmer.Vereinsname})" : teilnehmer.Name;
                BodyElements.Add(GetReciept(von));
            }
        }
        else if(_turnier.Wettbewerb is IContainerTeamBewerbe teamBewerb)
        {
            foreach(var team in teamBewerb.CurrentTeamBewerb.Teams)
            {
                BodyElements.Add(GetReciept(team.TeamName));
            }
        }
        
    }

    private Grid GetReciept(string von)
    {
        var grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };

        var receiptStackPanel = new StackPanel();
        receiptStackPanel.Children.Add(CutterLines.CutterLineTop());

        var receipt = new Receipt();
        receipt.labelAn.Content = _turnier.OrgaDaten.Organizer;
        receipt.labelVon.Content = von;
        receipt.labelEUR.Content = _turnier.OrgaDaten.EntryFee.Value.ToString("C");
        receipt.labelVerbal.Content = $"-- {_turnier.OrgaDaten.EntryFee.Verbal} --";
        receipt.labelZweck.Content = "Startgebühr";
        receipt.labelOrtDatum.Content = _turnier.OrgaDaten.Venue + ", " + _turnier.OrgaDaten.DateOfTournament.ToString("dd.MM.yyyy");
        receiptStackPanel.Children.Add(receipt);

        receiptStackPanel.Children.Add(CutterLines.CutterLine());

        grid.Children.Add(receiptStackPanel);
        return grid;
    }



    public List<Grid> BodyElements { get; private set; }
}
