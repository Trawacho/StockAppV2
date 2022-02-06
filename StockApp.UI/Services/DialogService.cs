using StockApp.UI.Dialogs;
using StockApp.UI.Stores;
using System;

namespace StockApp.UI.Services;

public class DialogService<TDialogModel> : IDialogService<TDialogModel>
        where TDialogModel : IDialogRequestClose, IDisposable
{
    private readonly IDialogStore _dialogStore;
    private readonly Func<TDialogModel> _createDialog;
    private readonly bool _asDialog;

    public DialogService(IDialogStore dialogService, Func<TDialogModel> createDialog, bool asDialog)
    {
        _dialogStore = dialogService;
        _createDialog = createDialog;
        _asDialog = asDialog;
    }

    public bool AsDialog => _asDialog;

    public bool? ShowDialog()
    {
        TDialogModel dialogModel = _createDialog();
        Type viewType = _dialogStore.Mappings[typeof(TDialogModel)];
        IDialog dialog = (IDialog)Activator.CreateInstance(viewType);
#pragma warning disable IDE0039 // Use local function
        EventHandler<DialogCloseRequestedEventArgs> handler = null;
#pragma warning restore IDE0039 // Use local function
        handler = (sender, e) =>
        {
            dialogModel.DialogCloseRequested -= handler;
            if (e.DialogResult.HasValue)
            {
                dialog.DialogResult = e.DialogResult;
            }
            else
            {
                dialog.Close();
            }
            dialogModel.Dispose();
        };

        dialogModel.DialogCloseRequested += handler;
        dialog.DataContext = dialogModel;
        dialog.Owner = _dialogStore.GetOwner();

        return dialog.ShowDialog();
    }

    public int OpenDialogs => _dialogStore?.OpenDialogCount ?? 0;

    public void Show()
    {
        TDialogModel dialogModel = _createDialog();
        Type viewType = _dialogStore.Mappings[typeof(TDialogModel)];
        IDialog dialog = (IDialog)Activator.CreateInstance(viewType);

#pragma warning disable IDE0039 // Use local function
        EventHandler<WindowCloseRequestedEventArgs> handler = null;
#pragma warning restore IDE0039 // Use local function
        handler = (sender, e) =>
        {
            _dialogStore.RemoveDialog(dialogModel);
            dialogModel.WindowCloseRequested -= handler;
            dialogModel.Dispose();
            dialog.Close();
        };

        dialogModel.WindowCloseRequested += handler;
        dialog.DataContext = dialogModel;
        dialog.Owner = _dialogStore.GetOwner();
        _dialogStore.AddDialog(dialogModel);
        dialog.Show();
    }
}



