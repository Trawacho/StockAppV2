using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Zielbewerb;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StockApp.Prints.ZielResult;
public class ZielResultViewModel : PrintTemplateViewModelBase
{
    private readonly ITurnier _turnier;
    private IZielBewerb ZielBewerb => _turnier.Wettbewerb as IZielBewerb;


    public ZielResultViewModel(ITurnier turnier) : base(turnier)
    {
        _turnier = turnier;
        InitBodyElements();
    }

    private void InitBodyElements()
    {
        var spielKlassen = ZielBewerb.Teilnehmerliste.Select(t => t.Spielklasse).Distinct();

        foreach (var klasse in spielKlassen)
        {
            if (spielKlassen.Count() > 1)
                BodyElements.Add(GetKlassenHeader(klasse, ZielBewerb.FontSize + 1));

            BodyElements.Add(GetGridTableHeader(ZielBewerb.FontSize, FontWeights.Bold, ZielBewerb.HasTeamname, ZielBewerb.HasNation));
            foreach (var row in GetGridPerPlayer(ZielBewerb, ZielBewerb.FontSize, ZielBewerb.RowSpace, ZielBewerb.HasTeamname, ZielBewerb.HasNation, klasse))
            {
                BodyElements.Add(row);
            }
        }
    }

    private static Grid GetKlassenHeader(string klasse, double fontSize)
    {
        var grid = new Grid();
        var label = new Label()
        {
            Content = klasse ??= $"nicht zugeordnet",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            FontSize = fontSize,
            FontWeight = FontWeights.Bold,
            Padding = new Thickness(0),
            Margin = new Thickness(0, 10, 0, 0)

        };
        grid.Children.Add(label);
        return grid;
    }
    public static Grid GetGridTemplate(bool showTeamName, bool showNation)
    {
        var grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };

        //Rang
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = GridLength.Auto,
            SharedSizeGroup = "A"
        });
        //Name
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = GridLength.Auto,
            SharedSizeGroup = "B"
        });
        //Verein
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = showTeamName ? GridLength.Auto : new GridLength(0),
            SharedSizeGroup = "C"
        });
        //Nation
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = showNation ? GridLength.Auto : new GridLength(0),
            SharedSizeGroup = "D"
        });
        //Platzhalter
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = new GridLength(1, GridUnitType.Star),
        });
        //GesamtPunkte
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = GridLength.Auto,
            SharedSizeGroup = "E"
        });
        //EinzelWertung
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = GridLength.Auto,
            SharedSizeGroup = "F"
        });


        grid.RowDefinitions.Add(new RowDefinition()
        {
            Height = GridLength.Auto
        });

        return grid;
    }
    public static Grid GetGridTableHeader(double fontSize, FontWeight fontWeight, bool showTeamName, bool showNation)
    {
        var grid = GetGridTemplate(showTeamName, showNation);

        var lblRang = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "Rang"
        };
        Grid.SetColumn(lblRang, 0);
        grid.Children.Add(lblRang);

        var lblSpieler = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(5, 0, 0, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Content = "Spieler"
        };
        Grid.SetColumn(lblSpieler, 1);
        grid.Children.Add(lblSpieler);

        var lblVerein = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(5, 0, 0, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Content = "Verein"
        };
        Grid.SetColumn(lblVerein, 2);
        grid.Children.Add(lblVerein);

        var lblNation = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(5, 0, 5, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "Nation"
        };
        Grid.SetColumn(lblNation, 3);
        grid.Children.Add(lblNation);

        var lblGesamt = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(5, 0, 5, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "Gesamt"
        };
        Grid.SetColumn(lblGesamt, 5);
        grid.Children.Add(lblGesamt);

        var lblEinzel = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(5, 0, 5, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "Detail"
        };
        Grid.SetColumn(lblEinzel, 6);
        grid.Children.Add(lblEinzel);



        return grid;

    }
    public static IEnumerable<Grid> GetGridPerPlayer(IZielBewerb bewerb, double fontSize, int rowSpace, bool showTeamName, bool showNation, string spielKlasse)
    {
        int i = 1;
        foreach (var player in bewerb.GetTeilnehmerRanked(spielKlasse))
        {
            Grid grid = GetGridTemplate(showTeamName, showNation);

            if (i % 2 == 0)
            {
                grid.Background = System.Windows.Media.Brushes.WhiteSmoke;
            }



            var lblRang = new Label()
            {
                Content = i,
                FontSize = fontSize,
                Padding = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            Grid.SetColumn(lblRang, 0);
            grid.Children.Add(lblRang);

            var lblName = new Label()
            {
                Content = player.Name,
                FontSize = fontSize,
                Padding = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(5, 0, 0, 0)
            };
            Grid.SetColumn(lblName, 1);
            grid.Children.Add(lblName);

            var lblTeam = new Label()
            {
                Content = player.Vereinsname,
                FontSize = fontSize,
                Padding = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(10, 0, 5, 0)
            };
            Grid.SetColumn(lblTeam, 2);
            grid.Children.Add(lblTeam);
            var lblNation = new Label()
            {
                Content = player.Nation,
                FontSize = fontSize,
                Padding = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetColumn(lblNation, 3);
            grid.Children.Add(lblNation);

            var lblGesamt = new Label()
            {
                Content = player.GesamtPunkte,
                FontSize = fontSize,
                Padding = new Thickness(0),
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5, 0, 5, 0)
            };
            Grid.SetColumn(lblGesamt, 5);
            grid.Children.Add(lblGesamt);

            // Einzelwertungen
            var einzelGrid = new Grid();
            foreach (var wertung in player.Wertungen)
            {
                einzelGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                var lblEinzel = new Label()
                {
                    Content = wertung.PunkteMassenMitte + "-" + wertung.PunkteSchuesse + "-" + wertung.PunkteMassenSeitlich + "-" + wertung.PunkteKombinieren,
                    FontSize = fontSize,
                    Padding = new Thickness(0),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5, 0, 5, 0)
                };
                Grid.SetRow(lblEinzel, einzelGrid.RowDefinitions.Count - 1);
                einzelGrid.Children.Add(lblEinzel);

            }
            Grid.SetColumn(einzelGrid, 6);
            grid.Children.Add(einzelGrid);


            if (rowSpace > 0)
            {
                var mainGrid = new Grid
                {
                    Background = grid.Background
                };
                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                var bottomGrid = new Grid();
                bottomGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowSpace, GridUnitType.Pixel) });

                var topGrid = new Grid();
                topGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowSpace, GridUnitType.Pixel) });

                Grid.SetRow(topGrid, 0);
                Grid.SetRow(grid, 1);
                Grid.SetRow(bottomGrid, 2);

                mainGrid.Children.Add(topGrid);
                mainGrid.Children.Add(grid);
                mainGrid.Children.Add(bottomGrid);
                yield return mainGrid;

            }
            else
            {
                yield return grid;
            }


            i++;

        }
    }

    public List<Grid> BodyElements { get; set; } = new List<Grid>();


    public static string HeaderString => $"E R G E B N I S";

    public static string Footer => null;

    public string Endtext => ZielBewerb.EndText;


    public string RefereeName => _turnier.OrgaDaten.Referee.Name;
    public string RefereeClub => _turnier.OrgaDaten.Referee.ClubName;
    public bool HasReferee => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Referee.Name);

    public string ComputingOfficerName => _turnier.OrgaDaten.ComputingOfficer.Name;
    public string ComputingOfficerClub => _turnier.OrgaDaten.ComputingOfficer.ClubName;
    public bool HasComputingOfficer => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.ComputingOfficer.Name);

    public string CompetitionManagerName => _turnier.OrgaDaten.CompetitionManager.Name;
    public string CompetitionManagerClub => _turnier.OrgaDaten.CompetitionManager.ClubName;
    public bool HasCompetitionManager => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.CompetitionManager.Name);

    public bool IsTitle2ndLine => ImageHeaderPath != null;
    public bool IsTitle1stLine => ImageHeaderPath == null;
    public string ImageTopLeftPath => ImageHeaderPath == null ? ZielBewerb.ImageTopLeftFileName : null;
    public string ImageTopRightPath => ImageHeaderPath == null ? ZielBewerb.ImageTopRightFileName : null;
    public string ImageHeaderPath => ZielBewerb.ImageHeaderFileName;

}
