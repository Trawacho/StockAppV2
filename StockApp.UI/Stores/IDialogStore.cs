using StockApp.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;

namespace StockApp.UI.Stores;
public interface IDialogStore
{
    void Register<TDialogModel, TView>() where TDialogModel : IDialogRequestClose, IDisposable
                                                    where TView : IDialog;
    void SetOwner(Window owner);
    Window GetOwner();

    void AddDialog<TDialogModel>(TDialogModel viewModel) where TDialogModel : IDialogRequestClose, IDisposable;
    void RemoveDialog<TDialogModel>(TDialogModel viewModel) where TDialogModel : IDialogRequestClose, IDisposable;

    void DisposeAllDialogs();

    IDictionary<Type, Type> Mappings { get; }

    int OpenDialogCount { get; }
}


