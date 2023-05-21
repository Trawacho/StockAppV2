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

public class TeamsViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private ITeamBewerb _currentBewerb;
    private TeamViewModel _selectedTeam;
    private readonly ICommand _addNewTeamCommand;
    private readonly ICommand _removeTeamCommand;
    private ICommand _modalOkCommand;
    private readonly ICommand _modalCancelCommand;
    private bool _isModalOpen;

    public ObservableCollection<TeamViewModel> Teams { get; }

    public TeamViewModel SelectedTeam
    {
        get => _selectedTeam;
        set => SetProperty(ref _selectedTeam, value);
    }

    private ITeamBewerb CurrentBewerb
    {
        get => _currentBewerb;
        set
        {
            if (_currentBewerb != null)
                _currentBewerb.TeamsChanged -= TeamsChanged;

            SetProperty(ref _currentBewerb, value);

            if (value != null)
                _currentBewerb.TeamsChanged += TeamsChanged;
        }
    }

    public ICommand AddTeamCommand => _addNewTeamCommand;
    public ICommand RemoveTeamCommand => _removeTeamCommand;
    public ICommand ModalOkCommand => _modalOkCommand;
    public ICommand ModalCancelCommand => _modalCancelCommand;

    public bool IsModalOpen
    {
        get => _isModalOpen;
        set => SetProperty(ref _isModalOpen, value);
    }

    private void AddTeam()
    {
        if (_currentBewerb?.Games.Any() ?? false)
        {
            IsModalOpen = true;

            _modalOkCommand = new RelayCommand(
                    (p) =>
                    {
                        IsModalOpen = false;
                        _currentBewerb?.AddNewTeam();
                    },
                    (p) => true);
            RaisePropertyChanged(nameof(ModalOkCommand));


        }
        else
        {
            _currentBewerb?.AddNewTeam();
            SelectedTeam = Teams?.Last();
        }
    }

    private void RemoveTeam()
    {
        if (_currentBewerb?.Games.Any() ?? false)
        {
            IsModalOpen = true;

            _modalOkCommand = new RelayCommand(
                (p) =>
                {
                    IsModalOpen = false;
                    _currentBewerb?.RemoveTeam(SelectedTeam.Team);
                },
                (p) => true);

            RaisePropertyChanged(nameof(ModalOkCommand));


        }
        else
        {
            _currentBewerb?.RemoveTeam(SelectedTeam.Team);
            SelectedTeam = Teams?.LastOrDefault();
        }
    }




    public TeamsViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        CurrentBewerb = _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb;

        Teams = new ObservableCollection<TeamViewModel>(CurrentBewerb.Teams.Select(s => new TeamViewModel(s)));

        _addNewTeamCommand = new RelayCommand((p) => AddTeam(), (p) => CurrentBewerb.Teams.Count() < turnierStore.MaxCountOfTeams);
        _removeTeamCommand = new RelayCommand((p) => RemoveTeam(), (p) => SelectedTeam != null);
        _modalCancelCommand = new RelayCommand(para => IsModalOpen = false);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                CurrentBewerb = null;
                SelectedTeam?.Dispose();
                SelectedTeam = null;
                Teams?.DisposeAndClear();
            }
            _disposed = true;
        }
    }

    private void TeamsChanged(object sender, EventArgs e)
    {
        Teams.DisposeAndClear();

        foreach (ITeam team in _currentBewerb.Teams)
            Teams.Add(new TeamViewModel(team));
    }
}
