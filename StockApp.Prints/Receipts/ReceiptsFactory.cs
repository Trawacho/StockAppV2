using StockApp.Core.Turnier;
using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Prints.BaseClasses;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace StockApp.Prints.Receipts
{
    public static class ReceiptsFactory
    {
        public static FixedDocument CreateReceipts(Size pageSize, ITurnier turnier)
        {
            return new ReceiptsHelper(pageSize, turnier).CreateReceipts();
        }

        internal class ReceiptsHelper : PrintsBaseClass
        {
            readonly ITurnier _turnier;
            internal ReceiptsHelper(Size pageSize, ITurnier turnier) : base(pageSize)
            {
                _turnier = turnier;
            }

            internal FixedDocument CreateReceipts()
            {
                var receipts = new List<StackPanel>();

                foreach (var team in (_turnier.Wettbewerb as ITeamBewerb).Teams.Where(t => !t.IsVirtual))
                {
                    receipts.Add(GetNewReceipt(team));
                }

                return base.CreateFixedDocument(receipts, true);
            }

            private StackPanel GetNewReceipt(ITeam team)
            {
                var receiptStackPanel = new StackPanel();
                receiptStackPanel.Children.Add(Components.CutterLineTop());

                var receipt = new Receipt();
                receipt.labelAn.Content = _turnier.OrgaDaten.Organizer;
                receipt.labelVon.Content = team.TeamName;
                receipt.labelEUR.Content = _turnier.OrgaDaten.EntryFee.Value.ToString("C");
                receipt.labelVerbal.Content = $"-- {_turnier.OrgaDaten.EntryFee.Verbal} --";
                receipt.labelZweck.Content = "Startgebühr";
                receipt.labelOrtDatum.Content = _turnier.OrgaDaten.Venue + ", " + _turnier.OrgaDaten.DateOfTournament.ToString("dd.MM.yyyy");
                receiptStackPanel.Children.Add(receipt);

                receiptStackPanel.Children.Add(Components.CutterLine());

                return receiptStackPanel;
            }
        }

    }
}
