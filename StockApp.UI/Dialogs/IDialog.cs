using System.Windows;

namespace StockApp.UI.Dialogs;
public interface IDialog
{
    object DataContext { get; set; }
    bool? DialogResult { get; set; }
    Window Owner { get; set; }
    void Close();
    bool? ShowDialog();

    void Show();
}

