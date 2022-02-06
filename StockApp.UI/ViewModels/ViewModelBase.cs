using Prism.Mvvm;
using System;

namespace StockApp.UI.ViewModels;

public class ViewModelBase : BindableBase, IDisposable
{
    internal bool _disposed = false;
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {

            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }

#if DEBUG
    ~ViewModelBase()
    {
        System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} deconstructed");
        Dispose(false);
    }
#endif
}
