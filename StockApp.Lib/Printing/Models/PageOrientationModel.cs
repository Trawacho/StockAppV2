using System.Printing;

namespace StockApp.Lib.Models;

public class PageOrientationModel
{
    public PageOrientationModel(PageOrientation pageOrientation)
    {
        PageOrientation = pageOrientation;
    }

    public PageOrientation PageOrientation { get; }

    public string PageOrientationName => PageOrientation.ToString();
}
