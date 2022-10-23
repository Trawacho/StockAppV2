using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Prints.BaseClasses;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace StockApp.Prints.Spielplan
{
    public static class SpielPlanFactory
    {
        public static FixedDocument CreateSpielPlan(Size pageSize, ITeamBewerb teamBewerb)
        {
            return new SpielPlanHelper(pageSize, teamBewerb).CreateSpielPlan();
        }
    }

    internal class SpielPlanHelper : PrintsBaseClass
    {
        private readonly ITeamBewerb _teamBewerb;

        internal SpielPlanHelper(Size pageSize, ITeamBewerb teamBewerb) : base(pageSize)
        {
            _teamBewerb = teamBewerb;
        }

        internal FixedDocument CreateSpielPlan()
        {
            var panel = new StackPanel() { HorizontalAlignment = HorizontalAlignment.Stretch };

            var header = new TextBlock()
            {
                Text = "Spielplan",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                FontWeight = FontWeights.Bold,
                FontSize = 30,
                Margin = new Thickness(30),
            };

            panel.Children.Add(header);

            var spielplan = new Spielplan(_teamBewerb.GetAllGames(false));
            var grid = new SpielPlanGrid(spielplan.Spiele);
            var vb = new Viewbox() { Child = grid, HorizontalAlignment = HorizontalAlignment.Stretch };
            panel.Children.Add(vb);

            return CreateFixedDocument(new StackPanel[] { panel }, true);
        }
    }


}

