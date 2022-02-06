using System.Windows;
using System.Windows.Controls;

namespace StockApp.UI.Components
{
    /// <summary>
    /// Interaction logic for TopHeader.xaml
    /// </summary>
    public partial class TopHeader : UserControl
    {
        public TopHeader()
        {
            InitializeComponent();
        }



        public string TopHeaderText
        {
            get { return (string)GetValue(TopHeaderTextProperty); }
            set { SetValue(TopHeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopHeaderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopHeaderTextProperty =
            DependencyProperty.Register("TopHeaderText", typeof(string), typeof(TopHeader), new PropertyMetadata("TopHeaderText undefined"));


    }
}
