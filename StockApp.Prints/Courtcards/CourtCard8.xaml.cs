using StockApp.Core.Wettbewerb.Teambewerb;
using System.Windows.Controls;

namespace StockApp.Prints.CourtCards
{
    /// <summary>
    /// Interaction logic for BahnBlock8.xaml
    /// </summary>
    public partial class BahnBlock8 : UserControl
    {
        internal BahnBlock8(IGame game)
        {
            InitializeComponent();

            this.BahnInfo.Content = $"Bahn: {game.CourtNumber}";
            this.SpielInfo.Content = $"Spiel: {game.GameNumber}";
            this.DurchgangInfo.Content = $"Runde: {game.RoundOfGame}";
            this.AnspielInfo.Content = $"Anspiel: {game.GetStartingTeam().StartNumber}";
            this.StartingTeamName.Content = $"{game.GetStartingTeam().TeamName}";
            this.StartTeamNumber.Content = $"{game.GetStartingTeam().StartNumber}";
            this.GegnerTeamName.Content = $"{game.GetNotStartingTeam().TeamName}";
            this.GegnerTeamNumber.Content = $"{game.GetNotStartingTeam().StartNumber}";
            this.UnterschriftGegnerTeam.Content = $"Unterschrift {game.GetNotStartingTeam().TeamName}";
            this.UnterschriftStartingTeam.Content = $"Unterschrift {game.GetStartingTeam().TeamName}";
        }
    }
}
