using Microsoft.Win32;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Dialogs;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;



namespace StockApp.UI.ViewModels;

public class LogViewerViewModel : ViewModelBase, IDialogRequestClose
{
	private readonly string _logFilePath;
	private FileSystemWatcher _watcher;
	private readonly SynchronizationContext _sync = SynchronizationContext.Current ?? new SynchronizationContext();

	public event EventHandler<DialogCloseRequestedEventArgs> DialogCloseRequested;
	public event EventHandler<WindowCloseRequestedEventArgs> WindowCloseRequested;
	protected virtual void RaiseCloseRequest(bool? dialogResult)
	{
		var dlgHandler = DialogCloseRequested;
		dlgHandler?.Invoke(this, new DialogCloseRequestedEventArgs(dialogResult));

		var wdwHandler = WindowCloseRequested;
		wdwHandler?.Invoke(this, new WindowCloseRequestedEventArgs());
	}

	public LogViewerViewModel(string logFilePath)
	{
		_logFilePath = logFilePath;

		LogFileName = Path.GetFileName(_logFilePath);

		RefreshCommand = new RelayCommand((p) => Refresh(), (p) => true);
		ExportCommand = new RelayCommand((p) => Export(), (p) => true);
		CloseCommand = new RelayCommand((p) => RaiseCloseRequest(null), (p) => true);

		SetupWatcher();
		Refresh();
	}

	// parameterless ctor kept for designer fallback (the view sets a design-time empty DataContext)
	public LogViewerViewModel()
	{
		_logFilePath = string.Empty;
		LogFileName = string.Empty;
		RefreshCommand = new RelayCommand((p) => { }, (p) => false);
		ExportCommand = new RelayCommand((p) => { }, (p) => false);
		CloseCommand = new RelayCommand((p) => { }, (p) => false);
	}

	public string LogFileName { get; init; }

	private string _logText = string.Empty;
	public string LogText
	{
		get => _logText;
		private set => SetProperty(ref _logText, value);
	}

	private bool _isFollowing = true;
	public bool IsFollowing
	{
		get => _isFollowing;
		set => SetProperty(ref _isFollowing, value);
	}

	public ICommand RefreshCommand { get; }
	public ICommand ExportCommand { get; }
	public ICommand CloseCommand { get; }

	// The view owner (caller) should set this to close the window without code-behind
	public Action CloseAction { get; set; }

	private void SetupWatcher()
	{
		try
		{
			if (string.IsNullOrEmpty(_logFilePath)) return;

			var dir = Path.GetDirectoryName(_logFilePath);
			var name = Path.GetFileName(_logFilePath);
			if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(name) || !Directory.Exists(dir))
				return;

			_watcher = new FileSystemWatcher(dir, name)
			{
				NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
			};
			_watcher.Changed += (s, e) => OnFileChanged();
			_watcher.Renamed += (s, e) => OnFileChanged();
			_watcher.EnableRaisingEvents = true;
		}
		catch
		{
			// best-effort only
		}
	}

	private void OnFileChanged()
	{
		// Debounce rapid events
		_sync.Post(async _ =>
		{
			await System.Threading.Tasks.Task.Delay(150);
			Refresh();
		}, null);
	}

	public void Refresh()
	{
		try
		{
			if (string.IsNullOrEmpty(_logFilePath) || !File.Exists(_logFilePath))
			{
				LogText = $"Log file not found: {_logFilePath}";
				return;
			}

			using var fs = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using var sr = new StreamReader(fs, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
			LogText = sr.ReadToEnd();
		}
		catch (Exception ex)
		{
			LogText = $"Error reading log: {ex.Message}";
		}
	}

	private void Export()
	{
		try
		{
			if (string.IsNullOrEmpty(_logFilePath) || !File.Exists(_logFilePath))
			{
				MessageBox.Show($"Log file not found: {_logFilePath}", "Export Log", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var dlg = new SaveFileDialog
			{
				FileName = LogFileName,
				Filter = "Log files (*.log;*.txt)|*.log;*.txt|All files (*.*)|*.*"
			};
			if (dlg.ShowDialog() != true)
				return;

			using var src = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			using var dst = new FileStream(dlg.FileName, FileMode.Create, FileAccess.Write, FileShare.None);
			src.CopyTo(dst);
			MessageBox.Show($"Log exported to '{dlg.FileName}'.", "Export Log", MessageBoxButton.OK, MessageBoxImage.Information);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Failed to export log: {ex.Message}", "Export Log", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_watcher?.Dispose();
				_watcher = null;
			}
			_disposed = true;
		}
		base.Dispose(disposing);
	}
}
