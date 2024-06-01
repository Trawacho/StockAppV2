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

namespace StockApp.Prints.Spielplan
{
    /// <summary>
    /// Interaction logic for SpielPlanTemplate.xaml
    /// </summary>
    public partial class SpielPlanTemplate : UserControl
    {
        public SpielPlanTemplate()
        {
            InitializeComponent();
        }
        public SpielPlanTemplate(ViewModelBase spielPlanTemplateViewModel) : this()
        {
            DataContext = spielPlanTemplateViewModel;
        }
    }
}
