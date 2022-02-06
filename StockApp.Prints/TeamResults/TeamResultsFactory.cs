using StockApp.Core.Turnier;
using StockApp.Prints.BaseClasses;
using StockApp.Prints.TeamResults;
using System.Windows;
using System.Windows.Documents;

namespace StockApp.Prints.Teamresult
{
    public static class TeamResultsFactory
    {
        public static FixedDocument CreateTeamResult(Size pageSize, ITurnier turnier)
        {
            return new TeamResultHelper(pageSize, turnier).CreateTeamResult();
        }
    }

    class TeamResultHelper : PrintsBaseClass
    {
        private readonly ITurnier _turnier;

        internal TeamResultHelper(Size pageSize, ITurnier turnier) : base(pageSize)
        {
            _turnier = turnier;
        }

        internal FixedDocument CreateTeamResult()
        {
            TeamResultPage page = new()
            {
                DataContext = new TeamResultPageViewModel(_turnier)
            };
            return CreateFixedDocument(page);
        }
    }
}
