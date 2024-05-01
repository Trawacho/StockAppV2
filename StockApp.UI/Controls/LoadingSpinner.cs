using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StockApp.UI.Controls;

public class LoadingSpinner : Control
{

    public static readonly DependencyProperty IsLoadingProperty =
   DependencyProperty.Register("IsLoading", typeof(bool), typeof(LoadingSpinner),
       new PropertyMetadata(false));

    public bool IsLoading
    {
        get { return (bool)GetValue(IsLoadingProperty); }
        set { SetValue(IsLoadingProperty, value); }
    }

    public static readonly DependencyProperty DiameterProperty =
        DependencyProperty.Register("Diameter", typeof(double), typeof(LoadingSpinner),
            new PropertyMetadata(100.0));

    public double Diameter
    {
        get { return (double)GetValue(DiameterProperty); }
        set { SetValue(DiameterProperty, value); }
    }

    public static readonly DependencyProperty ThicknessProperty =
        DependencyProperty.Register("Thickness", typeof(double), typeof(LoadingSpinner),
            new PropertyMetadata(1.0));

    public double Thickness
    {
        get { return (double)GetValue(ThicknessProperty); }
        set { SetValue(ThicknessProperty, value); }
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Brush), typeof(LoadingSpinner),
            new PropertyMetadata(Brushes.Black));

    public Brush Color
    {
        get { return (Brush)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public static readonly DependencyProperty CapProperty =
        DependencyProperty.Register("Cap", typeof(PenLineCap), typeof(LoadingSpinner),
            new PropertyMetadata(PenLineCap.Flat));

    public PenLineCap Cap
    {
        get { return (PenLineCap)GetValue(CapProperty); }
        set { SetValue(CapProperty, value); }
    }

    static LoadingSpinner()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(LoadingSpinner), new FrameworkPropertyMetadata(typeof(LoadingSpinner)));
    }


}
