using System.Windows;

namespace StockApp.UI.Settings;

public interface IWindowPlace
{

    //TODO: WindowPlace so umbauen, dass mehrere Fenster gespeichert werden.
    /*
     *  Beim laden immer prüfen, ob die Auflösung auch so zur Verfügung steht und ob das richtige Window gemeint ist
     * 
     */


    string Title { get; set; }
    Size VirtualScreenSize { get; set; }
    Rect WindowBounds { get; set; }
    WindowState WindowState { get; set; }

    string ToString();
}
