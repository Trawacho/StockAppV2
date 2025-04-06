using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace StockApp.UI.Behaviors;


public static class EventCommandBehavior
{
	public static readonly DependencyProperty EventProperty =
		DependencyProperty.RegisterAttached(
			"Event",
			typeof(string),
			typeof(EventCommandBehavior),
			new PropertyMetadata(null, OnEventChanged));

	public static string GetEvent(DependencyObject obj) =>
		(string)obj.GetValue(EventProperty);

	public static void SetEvent(DependencyObject obj, string value) =>
		obj.SetValue(EventProperty, value);

	public static readonly DependencyProperty CommandProperty =
		DependencyProperty.RegisterAttached(
			"Command",
			typeof(ICommand),
			typeof(EventCommandBehavior),
			new PropertyMetadata(null));

	public static ICommand GetCommand(DependencyObject obj) =>
		(ICommand)obj.GetValue(CommandProperty);

	public static void SetCommand(DependencyObject obj, ICommand value) =>
		obj.SetValue(CommandProperty, value);

	private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not UIElement uiElement || e.NewValue is not string eventName)
			return;

		// Hole EventInfo des angegebenen Events
		var eventInfo = d.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.Instance);
		if (eventInfo == null)
			throw new ArgumentException($"Event '{eventName}' not found on type '{d.GetType().Name}'.");

		// Dynamisch Handler erstellen
		var methodInfo = typeof(EventCommandBehavior).GetMethod(nameof(EventHandler), BindingFlags.NonPublic | BindingFlags.Static);
		var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType!, methodInfo!);

		eventInfo.RemoveEventHandler(d, handler); // doppelte Registrierung vermeiden
		eventInfo.AddEventHandler(d, handler);
	}

	public static readonly DependencyProperty CommandParameterProperty =
	DependencyProperty.RegisterAttached(
		"CommandParameter",
		typeof(object),
		typeof(EventCommandBehavior),
		new PropertyMetadata(null));

	public static object GetCommandParameter(DependencyObject obj) =>
		obj.GetValue(CommandParameterProperty);

	public static void SetCommandParameter(DependencyObject obj, object value) =>
		obj.SetValue(CommandParameterProperty, value);



	// Generischer Handler für Events
	private static void EventHandler(object? sender, EventArgs e)
	{
		if (sender is DependencyObject d)
		{
			var command = GetCommand(d);
			var parameter = GetCommandParameter(d);

			// Wenn kein Parameter explizit gesetzt, dann EventArgs verwenden
			var actualParameter = parameter ?? e;

			if (command?.CanExecute(actualParameter) == true)
				command.Execute(actualParameter);
		}
	}

}

