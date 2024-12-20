﻿using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.Prints.Receipts;
using StockApp.Prints.ScoreCards;
using StockApp.UI.com;
using StockApp.UI.Commands;
using StockApp.UI.Components;
using StockApp.UI.Settings;
using StockApp.UI.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class TeamViewModel : ViewModelBase
{

	private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


	private readonly ITeam _team;
	private readonly ITurnierStore _store;
	private TeamPlayersViewModel _teamPlayersViewModel;
	private readonly IEnumerable<string> _teamStatis;
	private readonly IEnumerable<IVerein> _vereine;

	public IEnumerable<string> TemplateVereine => _vereine?.Select(x => x.Name);
	
	public IEnumerable<string> TeamStatis => _teamStatis;

	public ITeam Team { get => _team; }

	public string StartNumber
	{
		get => _team.StartNumber.ToString(); set
		{
			if (int.TryParse(value, out int s))
			{
				_team.StartNumber = s;
			}
			RaisePropertyChanged();
		}
	}

	public string TeamName
	{
		get => _team.TeamName;
		set
		{
			_team.TeamName = value;
			RaisePropertyChanged();
		}
	}

	public string Nation
	{
		get => _team.Nation;
		set
		{
			_team.Nation = value;
			RaisePropertyChanged();
		}
	}
	public string Region
	{
		get => _team.Region;
		set
		{
			_team.Region = value;
			RaisePropertyChanged();
		}
	}

	public string Bundesland
	{
		get => _team.Bundesland;
		set
		{
			_team.Bundesland = value;
			RaisePropertyChanged();
		}
	}

	public string Kreis
	{
		get => _team.Kreis;
		set
		{
			_team.Kreis = value;
			RaisePropertyChanged();
		}
	}

	public string TeamStatus
	{
		get => _team.TeamStatus.Name();
		set
		{
			_team.TeamStatus = TeamStatusExtension.FromName(value);
			RaisePropertyChanged();
		}
	}

	public int StrafSpielpunkte
	{
		get => _team.StrafSpielpunkte;
		set
		{
			_team.StrafSpielpunkte = value;
			RaisePropertyChanged();
		}
	}

	public TeamPlayersViewModel TeamPlayersViewModel
	{
		get => _teamPlayersViewModel;
		set => SetProperty(ref _teamPlayersViewModel, value);
	}

	#region Constructor

	public TeamViewModel(ITeam team, ITurnierStore store)
	{
		_team = team;
		_store = store;
		TeamPlayersViewModel = new TeamPlayersViewModel(_team);
		_teamStatis = Enum.GetValues(typeof(TeamStatus))
						  .Cast<TeamStatus>()
						  .Select(x => x.Name());


		_vereine = store.TemplateVereine;
	}

	protected override void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				TeamPlayersViewModel.Dispose();
				_teamPlayersViewModel = null;
			}
			_disposed = true;
		}
	}

	#endregion Constructor

	#region Commands

	public ICommand TeamSelectedEnterCommand => new RelayCommand(
		(p) =>
		{
			var x = _vereine?.FirstOrDefault(v => v.Name == TeamName);
			if (x != null)
			{
				Nation = x.Land;
				Region = x.Region;
				Bundesland = x.Bundesland;
				Kreis = x.Kreis;
			}
		},
		(p) => true);

	private ICommand _printReceiptCommand;

	public ICommand PrintReceiptsCommand => _printReceiptCommand ??= new AsyncRelayCommand(
		async (p) =>
		{
			var _printPreview = new PrintPreview(await ReceiptsFactory.Create(_store.Turnier, _team.StartNumber));
			PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "Receipt");
			_printPreview.ShowDialog();
		},
		(p) => true);

	private ICommand _printWertungskarteCommand;

	public ICommand PrintWertungskarteCommand => _printWertungskarteCommand ??= new AsyncRelayCommand(
		async (p) =>
		{
			var _printPreview = new PrintPreview(
				await ScoreCardsFactory.Create(_store.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb
											 , _team.StartNumber
											 , PreferencesManager.TeamBewerbSettings.HasNamesOnScoreCard
											 , PreferencesManager.TeamBewerbSettings.HasSummarizedScoreCards
											 , PreferencesManager.TeamBewerbSettings.HasOpponentOnScoreCard));


			PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_printPreview, "WertungsKarte");
			_printPreview.ShowDialog();
		},
		(p) => _store.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.GetCountOfGames() > 0);
	
	#endregion Commands
}
