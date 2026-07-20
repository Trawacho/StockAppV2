using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StockApp.UI.Behaviors;

/// <summary>
/// Attached behavior: selects the entire text of a TextBox as soon as it receives keyboard focus
/// (Tab or Click). No code-behind required in the view.
/// </summary>
public static class TextBoxSelectAllOnFocusBehavior
{
	public static readonly DependencyProperty ActiveProperty =
		DependencyProperty.RegisterAttached(
			"Active",
			typeof(bool),
			typeof(TextBoxSelectAllOnFocusBehavior),
			new PropertyMetadata(false, OnActiveChanged));

	public static bool GetActive(DependencyObject obj) => (bool)obj.GetValue(ActiveProperty);
	public static void SetActive(DependencyObject obj, bool value) => obj.SetValue(ActiveProperty, value);

	private static void OnActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (d is not TextBox tb) return;

		bool was = (bool)e.OldValue;
		bool now = (bool)e.NewValue;

		if (was == now) return;

		if (now)
		{
			tb.GotKeyboardFocus += Tb_GotKeyboardFocus;
			tb.PreviewMouseLeftButtonDown += Tb_PreviewMouseLeftButtonDown;
		}
		else
		{
			tb.GotKeyboardFocus -= Tb_GotKeyboardFocus;
			tb.PreviewMouseLeftButtonDown -= Tb_PreviewMouseLeftButtonDown;
		}
	}

	private static void Tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
	{
		if (sender is TextBox tb)
			tb.SelectAll();
	}

	// Ohne diesen Handler würde WPF den Cursor nach dem Klick an die geklickte Stelle setzen
	// und damit die SelectAll()-Markierung aus GotKeyboardFocus sofort wieder aufheben.
	private static void Tb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
	{
		if (sender is not TextBox tb) return;

		if (!tb.IsKeyboardFocused)
		{
			e.Handled = true;
			tb.Focus();
		}
	}
}
