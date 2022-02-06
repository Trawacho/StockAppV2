using System;

namespace StockApp.UI.Dialogs;
public interface IDialogRequestClose
{
    event EventHandler<DialogCloseRequestedEventArgs> DialogCloseRequested;
    event EventHandler<WindowCloseRequestedEventArgs> WindowCloseRequested;
}
