using StockApp.Core.Turnier;
using StockApp.Lib.ViewModels;

namespace StockApp.UI.ViewModels;

public class EntryFeeViewModel : ViewModelBase, IEntryFeeViewModel
{
    private readonly IStartgebuehr _entryFee;
    public double Value
    {
        get => _entryFee.Value;
        set
        {
            _entryFee.Value = value;
            RaisePropertyChanged();
        }
    }

    public string Verbal
    {
        get => _entryFee.Verbal;
        set
        {
            _entryFee.Verbal = value;
            RaisePropertyChanged();
        }
    }

    public EntryFeeViewModel(IStartgebuehr entryFee)
    {
        _entryFee = entryFee;
    }
}

public class EntryFeeDesignViewModel : IEntryFeeViewModel
{
    public double Value { get; set; } = 10.0;
    public string Verbal { get; set; } = "zehn";
}

public interface IEntryFeeViewModel : IStartgebuehr
{

}
