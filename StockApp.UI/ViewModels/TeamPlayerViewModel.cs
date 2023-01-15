using StockApp.Core.Wettbewerb.Teambewerb;
using StockApp.Lib.ViewModels;

namespace StockApp.UI.ViewModels;

public class TeamPlayerViewModel : ViewModelBase
{
    private readonly IPlayer _player;
    public IPlayer Player => _player;
    public string FirstName
    {
        get => _player.FirstName;
        set
        {
            _player.FirstName = value;
            RaisePropertyChanged();
        }
    }

    public string LastName
    {
        get => _player.LastName; set
        {
            _player.LastName = value;
            RaisePropertyChanged();
        }
    }

    public string LicenseNumber
    {
        get => _player.LicenseNumber; set
        {
            _player.LicenseNumber = value;
            RaisePropertyChanged();
        }
    }

    public TeamPlayerViewModel(IPlayer player)
    {
        _player = player;
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {

            }
            _disposed = true;
        }
    }
}
