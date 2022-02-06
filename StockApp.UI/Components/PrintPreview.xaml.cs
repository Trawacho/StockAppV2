using System.Windows;
using System.Windows.Documents;

namespace StockApp.UI.Components
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        public PrintPreview(FixedDocument courtCards)
        {
            InitializeComponent();
            Document = courtCards;
        }
        public IDocumentPaginatorSource Document
        {
            get { return _viewer.Document; }
            set { _viewer.Document = value; }
        }
    }
}
