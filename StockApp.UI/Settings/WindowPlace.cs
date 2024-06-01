using System.Windows;

namespace StockApp.UI.Settings;

public class WindowPlace : IWindowPlace
{

    private Size _virtualScreenSize;
    private Rect _windowBounds;
    private WindowState _windowState;

    /// <summary>
    /// zur Verfügung stehende größe über alle Bildschirme
    /// </summary>
    public Size VirtualScreenSize { get => _virtualScreenSize; set => _virtualScreenSize = value; }
    /// <summary>
    /// Fenster von LEOn
    /// </summary>
    public Rect WindowBounds { get => _windowBounds; set => _windowBounds = value; }
    /// <summary>
    /// WindowState von LEOn
    /// </summary>
    public WindowState WindowState { get => _windowState; set => _windowState = value; }
    /// <summary>
    /// Name des Fensters
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Save size and position of window to file
    /// </summary>
    /// <param name="filePath">Name or path of XML file to save</param>
    internal WindowPlace()
    {
        WindowBounds = new Rect(0, 0, 1200, 900);
    }

    internal WindowPlace(string title, Size virtualScreenSize) : this()
    {
        Title = title;
        VirtualScreenSize = virtualScreenSize;
    }

    public override string ToString()
    {
        return $"Title:{Title}§Screen:{_virtualScreenSize}§Location:{_windowBounds.Location}§Size:{_windowBounds.Size}§State:{(int)WindowState}";
    }

}
