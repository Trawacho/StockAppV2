using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using StockApp.UI.Stores;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class TeamBewerbContainerViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private TeamBewerbViewModel _selectedTeamBewerb;

    #region Constructor
    public TeamBewerbContainerViewModel()
    {

    }

    public TeamBewerbContainerViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        TeamBewerbe =
            new ObservableCollection<TeamBewerbViewModel>(_turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Select(t => new TeamBewerbViewModel(t)));

        _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbeChanged += TeamBewerbe_TeamBewerbeChanged;
        SelectedTeamBewerb = TeamBewerbe?.First();
        GroupSelectorViewModel = new GroupSelectorViewModel(_turnierStore);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                GroupSelectorViewModel.Dispose();
                TeamBewerbe.DisposeAndClear();
                SelectedTeamBewerb?.Dispose();
                SelectedTeamBewerb = null;

                _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbeChanged -= TeamBewerbe_TeamBewerbeChanged;
            }
            _disposed = true;
        }
    }

    #endregion

    #region EventHandler
    private void TeamBewerbe_TeamBewerbeChanged(object sender, EventArgs e)
    {
        TeamBewerbe.Clear();
        foreach (ITeamBewerb teamBewerb in _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe)
        {
            TeamBewerbe.Add(new TeamBewerbViewModel(teamBewerb));
        }
        RaisePropertyChanged(nameof(IsGroupSelector));
    }
    #endregion

    #region Properties

    public ObservableCollection<TeamBewerbViewModel> TeamBewerbe { get; init; }

    public TeamBewerbViewModel SelectedTeamBewerb { get => _selectedTeamBewerb; set => SetProperty(ref _selectedTeamBewerb, value); }
    public ITeamBewerb ActiveTeamBewerb => _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;
    public bool IsGroupSelector => _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Count() > 1;
    public GroupSelectorViewModel GroupSelectorViewModel { get; init; }

    #endregion

    #region Commandos and Actions
    private void AddNewTeamBewerb()
    {
        _turnierStore.Turnier.ContainerTeamBewerbe.AddNew();
        RaisePropertyChanged(nameof(TeamBewerbe));
    }

    private void RemoveSelectedTeamBewerb()
    {
        _turnierStore.Turnier.ContainerTeamBewerbe.Remove(SelectedTeamBewerb.TeamBewerb);
        SelectedTeamBewerb = null;
        RaisePropertyChanged(nameof(TeamBewerbe));
    }

    public ICommand AddNewTeamBewerbCommand =>
        new RelayCommand(
            p => AddNewTeamBewerb(),
            p => TeamBewerbe.Count <= 3 && !_turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Any(t=>t.IsSplitGruppe));


    public ICommand RemoveSelectedTeamBewerbCommand =>
         new RelayCommand(
            p => RemoveSelectedTeamBewerb(),
            p => TeamBewerbe.Count > 1 && SelectedTeamBewerb != null && ActiveTeamBewerb.ID != SelectedTeamBewerb.ID);


    #endregion
}
