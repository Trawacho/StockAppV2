using StockApp.UI.Commands;
using StockApp.UI.Extensions;
using StockApp.UI.Stores;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class GroupSelectorViewModel : ViewModelBase
{
    private readonly ITurnierStore _turnierStore;
    private TeamBewerbViewModel _selectedTeamBewerb;
    private ICommand _setSelectedGroupCommand;
    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                SelectedTeamBewerb.Dispose();
                TeamBewerbe.DisposeAndClear();
                _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbeChanged += ContainerTeamBewerbe_TeamBewerbeChanged;
            }
            _disposed = true;
        }
    }

    public GroupSelectorViewModel()
    {

    }

    public GroupSelectorViewModel(ITurnierStore turnierStore)
    {
        _turnierStore = turnierStore;
        _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbeChanged += ContainerTeamBewerbe_TeamBewerbeChanged;
        ReloadData();
    }

    private void ContainerTeamBewerbe_TeamBewerbeChanged(object sender, EventArgs e) => ReloadData();
    private void ReloadData(bool onlySelectedTeamBewerb = false)
    {
        if (!onlySelectedTeamBewerb)
        {
            TeamBewerbe.Clear();
            foreach (var b in _turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe.Select(t => new TeamBewerbViewModel(t)))
            {
                TeamBewerbe.Add(b);
            }
        }
        SelectedTeamBewerb = TeamBewerbe.First(t => t.ID == _turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerb.ID);
    }

    private void SetSelectedGroup(object p)
    {
        if (p is TeamBewerbViewModel teamBewerb)
        {
            _turnierStore.Turnier.ContainerTeamBewerbe.SetCurrentTeamBewerb(teamBewerb.TeamBewerb);
            ReloadData(true);
        }
    }

    public ObservableCollection<TeamBewerbViewModel> TeamBewerbe { get; } = new();

    public TeamBewerbViewModel SelectedTeamBewerb
    {
        get => _selectedTeamBewerb;
        set => SetProperty(
                   storage: ref _selectedTeamBewerb,
                   value: value,
                   onChanged: () =>
                    {
                        if (value != null)
                            _turnierStore.Turnier.ContainerTeamBewerbe
                                .SetCurrentTeamBewerb(_turnierStore.Turnier.ContainerTeamBewerbe.TeamBewerbe
                                    .First(t => t.ID == value.ID));
                    });
    }


    public ICommand SelectGroupCommand => _setSelectedGroupCommand ??= new RelayCommand((p) => SetSelectedGroup(p), _ => true);


}
