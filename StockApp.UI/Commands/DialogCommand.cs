using StockApp.UI.Dialogs;
using StockApp.UI.Services;
using System;

namespace StockApp.UI.Commands;

public class DialogCommand<TDialogModel> : CommandBase
    where TDialogModel : IDialogRequestClose, IDisposable
{
    private readonly IDialogService<TDialogModel> _dialogService;

    public DialogCommand(IDialogService<TDialogModel> dlgService)
    {
        _dialogService = dlgService;
    }
    public override bool CanExecute(object parameter)
    {
        return _dialogService.OpenDialogs < 2;
    }

    public override void Execute(object parameter)
    {
        if (_dialogService.AsDialog)
        {
            _dialogService.ShowDialog();
        }
        else
        {
            _dialogService.Show();
        }
    }
}
