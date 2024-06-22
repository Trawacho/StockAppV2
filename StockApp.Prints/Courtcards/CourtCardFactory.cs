using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib;
using StockApp.Prints.BaseClasses;
using StockApp.Prints.Components;
using StockApp.Prints.Receipts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace StockApp.Prints.CourtCards
{
    public static class CourtCardFactory
    {
        public static FixedDocument CreateCourtCard(Size pageSize, ITeamBewerb teamBewerb)
        {
            return new CourtCardHelper(pageSize, teamBewerb).CreateCourtCards();
        }
    }

    internal class CourtCardHelper : PrintsBaseClass
    {
        readonly ITeamBewerb _teamBewerb;
        internal CourtCardHelper(Size pageSize, ITeamBewerb teamBewerb) : base(pageSize)
        {
            _teamBewerb = teamBewerb;
        }

        internal FixedDocument CreateCourtCards()
        {

            var allGames = _teamBewerb.GetAllGames(false);

            var courtCards = new List<StackPanel>();

            foreach (var game in allGames.OrderBy(b => b.CourtNumber).ThenBy(r => r.RoundOfGame).ThenBy(g => g.GameNumber))
            {
                courtCards.Add(CreateNewCourtCard(game));
            }


            return CreateFixedDocument(courtCards, true);
        }

        private StackPanel CreateNewCourtCard(IGame game)
        {
            var panel = new StackPanel();
            panel.Children.Add(CutterLines.CutterLineTop());

            if (_teamBewerb.Is8TurnsGame)
                panel.Children.Add(new BahnBlock8(game));
            else
                panel.Children.Add(new BahnBlock(game));

            panel.Children.Add(
                new TextBlock()
                {
                    Text = "created by StockApp",
                    FontSize = 7,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    HorizontalAlignment = HorizontalAlignment.Left
                });

            panel.Children.Add(CutterLines.CutterLine());

            return panel;
        }

    }
}
