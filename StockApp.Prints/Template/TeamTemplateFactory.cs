using StockApp.Core.Turnier;
using StockApp.Lib.PrintingComponents;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace StockApp.Prints.Template;

public static class TeamTemplateFactory
{
	public static async Task<IDocumentPaginatorSource> Create(ITurnier turnier)
	{
		UIElement reportFactory() => new TeamTemplate(new TeamTemplateViewModel(turnier));
		UIElement tableHeaderFactory() =>
			TeamTemplateViewModel.GetTableHeader(
				turnier.ContainerTeamBewerbe.CurrentTeamBewerb.FontSize,
				FontWeights.Bold,
				turnier.ContainerTeamBewerbe.CurrentTeamBewerb.TeamInfo);

		var helper = new PrintHelper();
		await helper.LoadReport(reportFactory, tableHeaderFactory, CancellationToken.None);
		return helper.GeneratedDocument;
	}
}
