using StockApp.Core.Factories;
using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.Models;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Converters;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace StockApp.Prints.Template;

internal class TeamTemplateViewModel : ViewModelBase
{
    private readonly ITurnier _turnier;
    private readonly ITeamBewerb _teamBewerb;
    private double _fontSize;

    public TeamTemplateViewModel()
    {
        HasOperator = true;
        HasOrganizer = true;
        IsBestOf = false;
        IsVergleich = false;
        _fontSize = 14;
    }

    public TeamTemplateViewModel(ITurnier turnier) : this()
    {
        _turnier = turnier;
        _teamBewerb = ((IContainerTeamBewerbe)turnier.Wettbewerb).CurrentTeamBewerb;

        HasOperator = !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Operator);
        HasOrganizer = !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Organizer);
        IsVergleich = GamePlanFactory.LoadAllGameplans().FirstOrDefault(g => g.ID == _teamBewerb.GameplanId)?.IsVergleich ?? false;
        IsBestOf = _teamBewerb.Teams.Count() == 2;
        RankedTeamsTableViewModels = new List<RankedTeamsTableViewModel>();
        foreach (var item in _turnier.ContainerTeamBewerbe.TeamBewerbe)
        {
            if (item.IsSplitGruppe)
            {
                RankedTeamsTableViewModels.Add(new RankedTeamsTableViewModel(item, isLive: false, isSplitGroupOne: true, showStockPunkte: true));
                RankedTeamsTableViewModels.Add(new RankedTeamsTableViewModel(item, isLive: false, isSplitGroupOne: false, showStockPunkte: true));
            }
            else
            {
                RankedTeamsTableViewModels.Add(new RankedTeamsTableViewModel(item, isLive: false, showGroupName: true));
            }
        }
        InitTeamRankingGrid();
    }

    public List<Grid> BodyElements { get; set; }

    private Grid GetGroupNameGrid(string groupName)
    {
        var grid = new Grid()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };
        var label = new Label()
        {
            FontSize = 18,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = groupName
        };
        grid.Children.Add(label);
        return grid;
    }

    private Grid GetGridTemplate()
    {
        var grid = new Grid() { HorizontalAlignment = HorizontalAlignment.Stretch };

        //AufAb Zeichen
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = new GridLength(PixelConverter.CmToPx(0.5)),
            SharedSizeGroup = "A"
        });
        //Rang
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = GridLength.Auto,
            SharedSizeGroup = "B"
        });
        //Teamname
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = new GridLength(1, GridUnitType.Star)
        });
        //Spielpunkte
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = GridLength.Auto,
            SharedSizeGroup = "D"
        });
        //Differenz
        grid.ColumnDefinitions.Add(new ColumnDefinition()
        {
            Width = GridLength.Auto,
            SharedSizeGroup = "E"
        });
        //Stockpunkte
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
    private Grid GetTableHeader(double fontSize, FontWeight fontWeight)
    {
        var grid = GetGridTemplate();

        var lblRang = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "Rang"
        };
        Grid.SetColumn(lblRang, 1);
        grid.Children.Add(lblRang);
        var lblTeam = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            Content = "Mannschaft"
        };
        Grid.SetColumn(lblTeam, 2);
        grid.Children.Add(lblTeam);
        var lblSpPkt = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "Punkte"
        };
        Grid.SetColumn(lblSpPkt, 3);
        grid.Children.Add(lblSpPkt);
        var lblDiff = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "Diff"
        };
        Grid.SetColumn(lblDiff, 4);
        grid.Children.Add(lblDiff);
        var lblStPkt = new Label()
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Margin = new Thickness(0, 0, 0, 0),
            FontSize = fontSize,
            FontWeight = fontWeight,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = "StockPunkte"
        };
        Grid.SetColumn(lblStPkt, 5);
        grid.Children.Add(lblStPkt);

        return grid;
    }

    private Grid GetTeamGridRow(RankedTeamModel rankedTeamModel, double fontSize, int rowSpace)
    {
        var teamGrid = GetGridTemplate();

        if (rankedTeamModel.Rank % 2 == 0)
        {
            teamGrid.Background = System.Windows.Media.Brushes.WhiteSmoke;
        }


        var lblAufAb = new Label() { Content = rankedTeamModel.AufAbSteiger, FontSize = fontSize / 2, Padding = new Thickness(0), VerticalContentAlignment = VerticalAlignment.Center };
        Grid.SetColumn(lblAufAb, 0);
        teamGrid.Children.Add(lblAufAb);

        var lblTeamRang = new Label() { Content = rankedTeamModel.Rank, FontSize = fontSize, Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center };
        Grid.SetColumn(lblTeamRang, 1);
        teamGrid.Children.Add(lblTeamRang);

        var lblTeamName = new Label() { Content = rankedTeamModel.TeamName, FontSize = fontSize, Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 0, 0, 0) };
        Grid.SetColumn(lblTeamName, 2);
        teamGrid.Children.Add(lblTeamName);

        var lblTeamSpielPunkte = new Label() { Content = rankedTeamModel.SpielPunkte, FontSize = fontSize, Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 0) };
        Grid.SetColumn(lblTeamSpielPunkte, 3);
        teamGrid.Children.Add(lblTeamSpielPunkte);

        var lblTeamDiff = new Label() { Content = rankedTeamModel.StockPunkteDifferenz, FontSize = fontSize, Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 0) };
        Grid.SetColumn(lblTeamDiff, 4);
        teamGrid.Children.Add(lblTeamDiff);

        var lblTeamStockPunkte = new Label() { Content = rankedTeamModel.StockPunkte, FontSize = fontSize, Padding = new Thickness(0), HorizontalContentAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 0, 0, 0) };
        Grid.SetColumn(lblTeamStockPunkte, 5);
        teamGrid.Children.Add(lblTeamStockPunkte);

        if (rankedTeamModel.HasPlayerNames)
        {
            var lblSpielernamen = new TextBlock()
            {
                Text = rankedTeamModel.PlayerNames,
                FontSize = fontSize * 0.75,
                FontStyle = FontStyles.Italic,
                FontWeight = FontWeights.Thin,
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Thickness(0),
                Margin = new Thickness(0, 0, 0, 0)

            };


            Grid.SetColumn(lblSpielernamen, 2);
            Grid.SetColumnSpan(lblSpielernamen, 4);
            Grid.SetRow(lblSpielernamen, 1);

            teamGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            teamGrid.Children.Add(lblSpielernamen);
        }

        if (rowSpace > 0)
        {
            var mainGrid = new Grid();
            mainGrid.Background = teamGrid.Background;
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var bottomGrid = new Grid();
            bottomGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowSpace, GridUnitType.Pixel) });

            var topGrid = new Grid();
            topGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(rowSpace, GridUnitType.Pixel) });

            Grid.SetRow(topGrid, 0);
            Grid.SetRow(teamGrid, 1);
            Grid.SetRow(bottomGrid, 2);

            mainGrid.Children.Add(topGrid);
            mainGrid.Children.Add(teamGrid);
            mainGrid.Children.Add(bottomGrid);
            return mainGrid;
        }


        return teamGrid;
    }

    private void InitTeamRankingGrid()
    {
        BodyElements = new List<Grid>();

        foreach (var rttvm in RankedTeamsTableViewModels)
        {
            BodyElements.Add(GetGroupNameGrid(rttvm.GroupName));

            BodyElements.Add(GetTableHeader(fontSize: _fontSize, fontWeight: FontWeights.Bold));

            foreach (var rt in rttvm.RankedTeams)
            {
                BodyElements.Add(GetTeamGridRow(rankedTeamModel: rt, fontSize: _fontSize, _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.RowSpace));
            }

        }
    }

    public List<RankedTeamsTableViewModel> RankedTeamsTableViewModels { get; init; }
    public ViewModelBase BestOfViewModel => IsBestOf ? new BestOfDetailViewModel(_teamBewerb, isLive: false) : default;
    public ViewModelBase RankedClubViewModel => IsVergleich ? new RankedClubTableViewModel(_teamBewerb, isLive: false) { AsDataGrid = false } : default;


    public string Title => _turnier.OrgaDaten.TournamentName;
    public bool IsTitle2ndLine => ImageHeaderPath != null;
    public bool IsTitle1stLine => ImageHeaderPath == null;

    public string Durchführer => _turnier.OrgaDaten.Operator;
    public bool HasOperator { get; init; }

    public string Veranstalter => _turnier.OrgaDaten.Organizer;
    public bool HasOrganizer { get; init; }

    public string Ort => _turnier.OrgaDaten.Venue;
    public string Datum => _turnier.OrgaDaten.DateOfTournament.ToString("dddd, dd.MM.yyyy");


    public string RefereeName => _turnier.OrgaDaten.Referee.Name;
    public string RefereeClub => _turnier.OrgaDaten.Referee.ClubName;
    public bool HasReferee => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.Referee.Name);

    public string ComputingOfficerName => _turnier.OrgaDaten.ComputingOfficer.Name;
    public string ComputingOfficerClub => _turnier.OrgaDaten.ComputingOfficer.ClubName;
    public bool HasComputingOfficer => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.ComputingOfficer.Name);

    public string CompetitionManagerName => _turnier.OrgaDaten.CompetitionManager.Name;
    public string CompetitionManagerClub => _turnier.OrgaDaten.CompetitionManager.ClubName;
    public bool HasCompetitionManager => !string.IsNullOrWhiteSpace(_turnier.OrgaDaten.CompetitionManager.Name);

    public bool IsVergleich { get; init; }



    public bool IsBestOf { get; init; }
    public bool HasMoreGroups => _turnier.ContainerTeamBewerbe.TeamBewerbe.Count() > 1 || _turnier.ContainerTeamBewerbe.TeamBewerbe.Where(b => b.IsSplitGruppe).Any();
    public string HeaderString => _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.IsEachGameDone(false) ? $"E R G E B N I S" : "Zwischenergebnis";

    public string Endtext => _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Endtext;
    public string Footer
    {
        get
        {
            var footerText = string.Empty;

            if (_turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Teams.Any(t => t.TeamStatus != TeamStatus.Normal))
                footerText = TeamStatusExtension.FooterText();

            if (_turnier.ContainerTeamBewerbe.CurrentTeamBewerb.Teams.Any(t => t.StrafSpielpunkte > 0))
            {
                if (!string.IsNullOrEmpty(footerText))
                    footerText += "; ";

                footerText += "$ = Spielpunktstrafe";
            }

            return footerText;
        }
    }


    public string ImageTopLeftPath => ImageHeaderPath == null ? _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopLeftFilename : null;
    public string ImageTopRightPath => ImageHeaderPath == null ? _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageTopRightFilename : null;
    public string ImageHeaderPath => _turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ImageHeaderFilename;
}
