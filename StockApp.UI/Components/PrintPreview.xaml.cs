using System.Windows;
using System.Windows.Documents;

namespace StockApp.UI.Components
{
    /// <summary>
    /// Interaction logic for PrintPreview.xaml
    /// </summary>
    public partial class PrintPreview : Window
    {
        public PrintPreview()
        {
            InitializeComponent();
        }

        public PrintPreview(FixedDocument courtCards) : this()
        {
            Document = courtCards;
        }

        public PrintPreview(IDocumentPaginatorSource paginatorSource) : this()
        {
            Document = paginatorSource;
        }

        public IDocumentPaginatorSource Document
        {
            get { return _viewer.Document; }
            set { _viewer.Document = value; }
        }
    }
}
