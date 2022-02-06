using StockApp.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows;

namespace StockApp.UI.Stores;
public class DialogStore : IDialogStore
{
    private Window _owner;
    private readonly IList<IDisposable> _openDialogs;

    public DialogStore(Window owner)
    {
        this._owner = owner;
        Mappings = new Dictionary<Type, Type>();
        _openDialogs = new List<IDisposable>();
    }

    public void SetOwner(Window owner)
    {
        this._owner = owner;
    }
    public Window GetOwner() => _owner;
    public IDictionary<Type, Type> Mappings { get; }
    public int OpenDialogCount => _openDialogs.Count;
    public void Register<TDialogModel, TView>() where TDialogModel : IDialogRequestClose, IDisposable
                                                    where TView : IDialog
    {
        if (Mappings.ContainsKey(typeof(TDialogModel)))
        {
            throw new ArgumentException($"$Type {typeof(TDialogModel)} is already mapped to type {typeof(TView)}");
        }

        Mappings.Add(typeof(TDialogModel), typeof(TView));
    }

    public void DisposeAllDialogs()
    {
        foreach (var item in _openDialogs)
        {
            item.Dispose();
        }
    }

    public void AddDialog<TDialogModel>(TDialogModel dialogModel) where TDialogModel : IDialogRequestClose, IDisposable
    {
        _openDialogs.Add(dialogModel);
    }

    public void RemoveDialog<TDialogModel>(TDialogModel dialogModel) where TDialogModel : IDialogRequestClose, IDisposable
    {
        _openDialogs.Remove(dialogModel);
    }

}
