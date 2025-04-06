using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace StockApp.UI.Behaviors;

public static class WindowLifecycleBehavior
{
	#region LoadedCommand
	public static readonly DependencyProperty LoadedCommandProperty =
		DependencyProperty.RegisterAttached(
			"LoadedCommand",
			typeof(ICommand),
			typeof(WindowLifecycleBehavior),
			new PropertyMetadata(null, OnLoadedCommandChanged));

	public static ICommand GetLoadedCommand(DependencyObject obj) =>
		(ICommand)obj.GetValue(LoadedCommandProperty);

	public static void SetLoadedCommand(DependencyObject obj, ICommand value) =>
		obj.SetValue(LoadedCommandProperty, value);

	private static void OnLoadedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is Window window)
		{
			window.Loaded -= OnWindowLoaded;
			window.Loaded += OnWindowLoaded;
		}
	}

	private static void OnWindowLoaded(object sender, RoutedEventArgs e)
	{
		if (sender is Window window)
		{
			var command = GetLoadedCommand(window);
			if (command?.CanExecute(null) == true)
				command.Execute(null);
		}
	}
	#endregion

	#region ClosingCommand
	public static readonly DependencyProperty ClosingCommandProperty =
	   DependencyProperty.RegisterAttached(
		   "ClosingCommand",
		   typeof(ICommand),
		   typeof(WindowLifecycleBehavior),
		   new PropertyMetadata(null, OnClosingCommandChanged));

	public static ICommand GetClosingCommand(DependencyObject obj) =>
		(ICommand)obj.GetValue(ClosingCommandProperty);

	public static void SetClosingCommand(DependencyObject obj, ICommand value) =>
		obj.SetValue(ClosingCommandProperty, value);

	private static void OnClosingCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is Window window)
		{
			window.Closing -= OnWindowClosing;
			window.Closing += OnWindowClosing;
		}
	}

	private static void OnWindowClosing(object sender, CancelEventArgs e)
	{
		if (sender is Window window)
		{
			var command = GetClosingCommand(window);
			if (command?.CanExecute(null) == true)
			{
				var args = new CancelCommandParameter { Cancel = false };
				command.Execute(args);
				e.Cancel = args.Cancel;
			}
		}
	}
	#endregion
}

// Hilfsklasse für Cancel-Logik
public class CancelCommandParameter
{
	public bool Cancel { get; set; }
}
