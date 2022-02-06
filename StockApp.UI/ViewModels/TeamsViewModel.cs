using StockApp.Core.Wettbewerb.Teambewerb;
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
    private readonly ITeamBewerb _currentBewerb;
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
        _currentBewerb = _turnierStore.Turnier.Wettbewerb as ITeamBewerb;

        Teams = new ObservableCollection<TeamViewModel>(_currentBewerb.Teams.Where(t => !t.IsVirtual).Select(s => new TeamViewModel(s)));
        _currentBewerb.TeamsChanged += TeamsChanged;

        _addNewTeamCommand = new RelayCommand((p) => AddTeam(), (p) => _currentBewerb.Teams.Count() < 15);
        _removeTeamCommand = new RelayCommand((p) => RemoveTeam(), (p) => SelectedTeam != null);
        _modalCancelCommand = new RelayCommand(para => IsModalOpen = false);
    }


    private void TeamsChanged(object sender, EventArgs e)
    {
        Teams.DisposeAndClear();

        foreach (ITeam team in _currentBewerb.Teams.Where(t => !t.IsVirtual))
            Teams.Add(new TeamViewModel(team));
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _currentBewerb.TeamsChanged -= TeamsChanged;
                SelectedTeam = null;
                Teams?.DisposeAndClear();
            }
            _disposed = true;
        }
    }
}
