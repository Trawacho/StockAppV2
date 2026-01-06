using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace StockApp.UI.Behaviors;

/// <summary>
/// Attached behavior to auto-scroll a TextBox to end when its text changes and Follow is true.
/// No code-behind required in the view.
/// </summary>
public static class TextBoxAutoScrollBehavior
{
	public static readonly DependencyProperty FollowProperty =
		DependencyProperty.RegisterAttached(
			"Follow",
			typeof(bool),
			typeof(TextBoxAutoScrollBehavior),
			new PropertyMetadata(false, OnFollowChanged));

	public static bool GetFollow(DependencyObject obj) => (bool)obj.GetValue(FollowProperty);
	public static void SetFollow(DependencyObject obj, bool value) => obj.SetValue(FollowProperty, value);

	private static void OnFollowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not TextBox tb) return;

		bool was = (bool)e.OldValue;
		bool now = (bool)e.NewValue;

		if (was == now) return;

		if (now)
		{
			tb.TextChanged += Tb_TextChanged;
			// optionally scroll immediately to end
			ScrollToEndSafe(tb);
		}
		else
		{
			tb.TextChanged -= Tb_TextChanged;
		}
	}

	private static void Tb_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (sender is TextBox tb && GetFollow(tb))
			ScrollToEndSafe(tb);
	}

	private static void ScrollToEndSafe(TextBox tb)
	{
		try
		{
			// ensure scroll happens on UI thread and after layout update
			tb.Dispatcher.BeginInvoke((Action)(() =>
			{
				try
				{
					tb.CaretIndex = tb.Text?.Length ?? 0;
					tb.ScrollToEnd();
				}
				catch { /* swallow UI timing errors */ }
			}), DispatcherPriority.Background);
		}
		catch { }
	}
}
