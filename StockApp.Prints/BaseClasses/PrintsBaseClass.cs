using StockApp.Prints.Converters;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace StockApp.Prints.BaseClasses;

internal abstract class PrintsBaseClass
{
    private readonly FixedDocument _document;
    internal PrintsBaseClass(Size pageSize)
    {
        _document = new FixedDocument();
        _document.DocumentPaginator.PageSize = pageSize;
    }

    protected FixedDocument CreateFixedDocument(IEnumerable<StackPanel> panels, bool setLeftSpace)
    {
        var pagePanel = new StackPanel();

        foreach (var p in panels)
        {
            pagePanel.Children.Add(p);
            pagePanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            pagePanel.Arrange(new Rect(0, 0, pagePanel.DesiredSize.Width, pagePanel.DesiredSize.Height));

            if (pagePanel.ActualHeight + p.ActualHeight > _document.DocumentPaginator.PageSize.Height)
            {
                SetPagePanelToDocument(pagePanel, setLeftSpace);
                pagePanel = new StackPanel();
            }
        }

        if (pagePanel.Children.Count > 0)
        {
            SetPagePanelToDocument(pagePanel, setLeftSpace);
        }

        return _document;
    }

    protected FixedDocument CreateFixedDocument(UserControl control)
    {
        var newPage = GetNewPage();

        newPage.HorizontalAlignment = HorizontalAlignment.Center;
        newPage.VerticalAlignment = VerticalAlignment.Center;

        //Wenn die aktuelle Höhe + die neue Höhe > seiten-Höhe
        // FixedPage.SetTop(control, PixelConverter.CmToPx(0));
        // FixedPage.SetLeft(control, PixelConverter.CmToPx(0));
        newPage.Children.Add(control);

        AddNewPageToDocument(newPage);

        return _document;
    }


    private protected void SetPagePanelToDocument(StackPanel panel, bool setLeftSpace)
    {

        double leftSpaceCm = setLeftSpace ? 2.0 : 0.7;

        var newPage = GetNewPage();

        //Wenn die aktuelle Höhe + die neue Höhe > seiten-Höhe
        FixedPage.SetTop(panel, PixelConverter.CmToPx(1));
        FixedPage.SetLeft(panel, PixelConverter.CmToPx(leftSpaceCm));
        newPage.Children.Add(panel);

        AddNewPageToDocument(newPage);

    }

    private void AddNewPageToDocument(FixedPage newPage)
    {
        var content = new PageContent();
        ((IAddChild)content).AddChild(newPage);
        _document.Pages.Add(content);
    }

    private FixedPage GetNewPage()
    {
        return new FixedPage
        {
            Width = _document.DocumentPaginator.PageSize.Width,
            Height = _document.DocumentPaginator.PageSize.Height
        };
    }
}

