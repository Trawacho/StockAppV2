using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Components;
using StockApp.Prints.ScoreCards.Base;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace StockApp.Prints.ScoreCards;

public class ScoreCardTemplateViewModel : ViewModelBase
{
	public List<Grid> BodyElements { get; private set; }

	public ScoreCardTemplateViewModel(ITeamBewerb bewerb, bool namesOnScoreCard, bool summarizeScoreCards, bool opponentOnScoreCard, bool asCupCard)
									: this(bewerb, 0, namesOnScoreCard, summarizeScoreCards, opponentOnScoreCard, asCupCard) { }
	public ScoreCardTemplateViewModel(ITeamBewerb bewerb, int startNummer, bool namesOnScoreCard, bool summarizeScoreCards, bool opponentOnScoreCard, bool asCupCard)
	{
		InitBodyElements(bewerb, startNummer, namesOnScoreCard, summarizeScoreCards, opponentOnScoreCard, asCupCard);
	}

	private void InitBodyElements(ITeamBewerb bewerb, int startNummer, bool namesOnScoreCard, bool summarizeScoreCards, bool opponentOnScoreCard, bool asCupCard)
	{
		BodyElements = new();
		if (startNummer == 0)
		{
			foreach (var team in bewerb.Teams.OrderBy(t => t.StartNumber))
			{
				GetScoreCard(team
							, namesOnScoreCard
							, bewerb.Is8TurnsGame
							, summarizeScoreCards
							, opponentOnScoreCard
							, asCupCard);
			}
		}
		else
		{
			GetScoreCard(bewerb.Teams.Single(t => t.StartNumber == startNummer)
						, namesOnScoreCard
						, bewerb.Is8TurnsGame
						, summarizeScoreCards
						, opponentOnScoreCard
						, asCupCard);
		}
	}

	private void GetScoreCard(ITeam team, bool namesOnScoreCard, bool is8TurnsGame, bool summarizeScoreCards, bool opponentOnScoreCards, bool asCupCard)
	{
		if (asCupCard)
		{
			if (summarizeScoreCards)
			{
				BodyElements.Add(GetScoreCardGrid(
					team,
					team.GetGamesOrderedForCupModus(),
					namesOnScoreCard,
					is8TurnsGame,
					0,
					opponentOnScoreCards,
					"Gegner"
					));
			}
			else
			{
				foreach (var g in team.Games.Where(g => !g.IsPauseGame()).Where(t => t.TeamA.StartNumber == team.StartNumber).GroupBy(g => g.TeamB))
				{
					BodyElements.Add(GetScoreCardGrid(
						team,
						g.OrderBy(g => g.GameNumberOverAll),
						namesOnScoreCard,
						is8TurnsGame,
						0,
						opponentOnScoreCards,
						g.First().TeamB.TeamName
						));
				}
			}
		}

		else if (summarizeScoreCards)
		{
			BodyElements.Add(GetScoreCardGrid(
				team,
				team.GetGamesOrderedByGameNumberOverAll(),
				namesOnScoreCard,
				is8TurnsGame,
				0,
				opponentOnScoreCards,
				"Gegner"
				));
		}

		else
		{
			int maxRounds = team.Games.Max(r => r.RoundOfGame);
			for (int i = 1; i <= maxRounds; i++)
			{
				BodyElements.Add(GetScoreCardGrid(
					team,
					team.Games.Where(g => g.RoundOfGame == i).OrderBy(o => o.GameNumber),
					namesOnScoreCard,
					is8TurnsGame,
					maxRounds == 1 ? 0 : i,
					opponentOnScoreCards,
					"Gegner"
					));
			}

		}
	}

	private Grid GetScoreCardGrid(ITeam team, IEnumerable<IGame> games, bool namesOnScoreCard, bool is8TurnsGame, int numberOfGameRound, bool opponentOnScoreCards, string nameGegner)
	{
		var grid = new Grid();
		var panel = new StackPanel();
		panel.Children.Add(CutterLines.CutterLineTop());
		panel.Children.Add(new ScoreCardHeader(team.StartNumber, team.TeamName, namesOnScoreCard, is8TurnsGame, numberOfGameRound, opponentOnScoreCards, nameGegner));
		panel.Children.Add(new ScoreCardHeaderGrid(is8TurnsGame, opponentOnScoreCards));
		foreach (var game in games)
		{
			panel.Children.Add(new GameGrid(game, team, is8TurnsGame, opponentOnScoreCards));
		}
		panel.Children.Add(new GameSummaryGrid(is8TurnsGame, opponentOnScoreCards));
		panel.Children.Add(CutterLines.CutterLine());
		grid.Children.Add(panel);
		return grid;
	}
}