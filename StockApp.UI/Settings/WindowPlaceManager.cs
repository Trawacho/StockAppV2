using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace StockApp.UI.Settings;

public class WindowPlaceManager
{
    private readonly HashSet<IWindowPlace> _windowsPlaces = new();
    public IEnumerable<IWindowPlace> WindowPlaces => _windowsPlaces;

    public void Register(Window window, string windowTitle)
    {
        var thisPlace = _windowsPlaces?.FirstOrDefault(p =>
                                                       p.Title == windowTitle &&
                                                       p.VirtualScreenSize.Height == SystemParameters.VirtualScreenHeight &&
                                                       p.VirtualScreenSize.Width == SystemParameters.VirtualScreenWidth)
                            ?? Add(new WindowPlace(
                                        title: windowTitle,
                                        virtualScreenSize: new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight)));

        window.Loaded += (o, e) =>
        {
            window.Width = thisPlace.WindowBounds.Width;
            window.Height = thisPlace.WindowBounds.Height;
            window.Left = SystemParameters.VirtualScreenLeft < thisPlace.WindowBounds.Left ? thisPlace.WindowBounds.Left : SystemParameters.VirtualScreenLeft;
            window.Top = SystemParameters.VirtualScreenTop < thisPlace.WindowBounds.Top ? thisPlace.WindowBounds.Top : SystemParameters.VirtualScreenTop;
            window.WindowState = thisPlace.WindowState == WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
        };

        window.Closing += (o, e) =>
        {
            if (!e.Cancel)
            {
                thisPlace.VirtualScreenSize = new Size(SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
                thisPlace.WindowBounds = new Rect(window.Left, window.Top, window.Width, window.Height);
                thisPlace.WindowState = window.WindowState;
            }
        };
    }




    internal static IWindowPlace Convert(string windowPlaceString)
    {
        var windowPlace = new WindowPlace();

        try
        {
            string[] a = windowPlaceString.Split('§');
            windowPlace = new()
            {
                Title = a[0].Split(':')[1],

                VirtualScreenSize = new Size()
                {
                    Width = int.Parse(a[1].Split(':')[1].Split(';')[0]),
                    Height = int.Parse(a[1].Split(':')[1].Split(';')[1])
                },

                WindowBounds = new Rect()
                {
                    X = int.Parse(a[2].Split(':')[1].Split(';')[0]),
                    Y = int.Parse(a[2].Split(':')[1].Split(';')[1]),
                    Width = int.Parse(a[3].Split(':')[1].Split(';')[0]),
                    Height = int.Parse(a[3].Split(':')[1].Split(';')[1]),
                },

                WindowState = (WindowState)int.Parse(a[4].Split(':')[1])
            };
        }
        catch
        {

        }

        return windowPlace;
    }
    internal IWindowPlace Add(IWindowPlace windowPlace)
    {
        if (!_windowsPlaces.Any(p =>
                        p.VirtualScreenSize == windowPlace.VirtualScreenSize
                     && p.Title == windowPlace.Title))
        {
            _windowsPlaces.Add(windowPlace);

        }

        return windowPlace;
    }
}
