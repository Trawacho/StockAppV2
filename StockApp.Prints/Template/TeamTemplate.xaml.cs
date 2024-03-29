using StockApp.Lib.ViewModels;
using System.Windows.Controls;

namespace StockApp.Prints.Template
{
    /// <summary>
    /// Interaction logic for TeamTemplate.xaml
    /// </summary>
    public partial class TeamTemplate : UserControl
    {
        public TeamTemplate()
        {
            InitializeComponent();
        }

        public TeamTemplate(ViewModelBase teamTemplateViewModel):this()
        {
                DataContext = teamTemplateViewModel;
        }
    }
}
