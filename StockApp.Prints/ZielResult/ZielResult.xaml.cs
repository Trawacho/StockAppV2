using StockApp.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockApp.Prints.ZielResult;

/// <summary>
/// Interaction logic for ZielResult.xaml
/// </summary>
public partial class ZielResult : UserControl
{
    public ZielResult()
    {
        InitializeComponent();
    }

    public ZielResult(ViewModelBase zielResultViewModel) : this()
    {
        DataContext = zielResultViewModel;
    }


}
