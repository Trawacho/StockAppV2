using StockApp.UI.Dialogs;
using System;

namespace StockApp.UI.Services
{
    public interface IDialogService<TDialogModel>
        where TDialogModel : IDialogRequestClose, IDisposable
    {
        void Show();
        bool? ShowDialog();
        bool AsDialog { get; }

        int OpenDialogs { get; }
    }
}