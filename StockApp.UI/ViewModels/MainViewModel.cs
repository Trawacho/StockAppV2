using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using StockApp.Lib.ViewModels;
using StockApp.UI.Commands;
using StockApp.UI.Settings;
using StockApp.UI.Stores;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace StockApp.UI.ViewModels;

public class MainViewModel : ViewModelBase
{
	#region Fields

	private readonly INavigationStore _navigationStore;
	private readonly ITurnierStore _turnierStore;
	private bool _isModalOpen;
	private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	#endregion

	#region RequestClose [Event]

	public event EventHandler<CancelEventArgs> RequestClose;
	private void OnRequestClose(CancelEventArgs e)
	{
		var handler = RequestClose;
		handler?.Invoke(this, e);
	}
	private void RaiseOnRequestClose(CancelEventArgs e)
	{
		if (_turnierStore.IsDuty())
		{
			IsModalOpen = true;

			if (e != null)
				e.Cancel = true;
		}
		else
		{
			OnRequestClose(e);
		}
	}

	#endregion

	#region Constructor

	public MainViewModel()
	{

	}

	public MainViewModel(INavigationViewModel navigationViewModel, INavigationStore navigationStore, ITurnierStore turnierStore)
	{
		_navigationStore = navigationStore;
		_turnierStore = turnierStore;

		NewTournamentCommand = new RelayCommand(
			(p) =>
			{
				_turnierStore.Turnier?.Reset();
				NavigationViewModel.NavigationReset();
			});

		SaveAsTournamentCommand = new RelayCommand((p) => _turnierStore.SaveAs());
		OpenTournamentCommand = new RelayCommand(
			(p) =>
			{
				_turnierStore.Load();
				NavigationViewModel.NavigationReset();
			});
		SaveTournamentCommand = new RelayCommand((p) => _turnierStore.Save());

		ExitApplicationCommand = new RelayCommand((p) => RaiseOnRequestClose(null));
		ClosingCommand = new RelayCommand<CancelEventArgs>((p) => RaiseOnRequestClose(p));

		ModalOkCommand = new RelayCommand(
			(p) =>
			{
				IsModalOpen = false;
				OnRequestClose(null);
			});
		ModalCancelCommand = new RelayCommand(
			(p) =>
			{
				IsModalOpen = false;
			});


		_turnierStore.FileNameChanged += TurnierStore_FileNameChanged;
		_navigationStore.CurrentViewModelChanged += CurrentViewModelChanged;
		_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged += ContainerTeamBewerbe_ActiveTeamBewerbChanged;

		NavigationViewModel = navigationViewModel;
	}
	protected override void Dispose(bool disposing)
	{
		if (!_disposed)
		{
			if (disposing)
			{
				_turnierStore.FileNameChanged -= TurnierStore_FileNameChanged;
				_navigationStore.CurrentViewModelChanged -= CurrentViewModelChanged;
				_turnierStore.Turnier.ContainerTeamBewerbe.CurrentTeamBewerbChanged -= ContainerTeamBewerbe_ActiveTeamBewerbChanged;
			}
			_disposed = true;
		}
		base.Dispose(disposing);
	}

	#endregion

	#region RaisePropertyChanged

	private void TurnierStore_FileNameChanged(object sender, EventArgs e)
	{
		RaisePropertyChanged(nameof(FileName));
	}

	private void CurrentViewModelChanged(object sender, EventArgs e)
	{
		RaisePropertyChanged(nameof(CurrentViewModel));
	}

	private void ContainerTeamBewerbe_ActiveTeamBewerbChanged(object sender, EventArgs e)
	{
		RaisePropertyChanged(nameof(ActiveTeamBewerbGroupName));
	}

	#endregion



	#region Properties

	public bool IsModalOpen
	{
		get => _isModalOpen;
		set
		{
			_isModalOpen = value;
			RaisePropertyChanged();
		}
	}

	public Level LogLevel
	{
		get => PreferencesManager.GeneralAppSettings.LogLevel;
		set
		{
			PreferencesManager.GeneralAppSettings.LogLevel = value;
			RaisePropertyChanged();
		}
	}

	public ViewModelBase CurrentViewModel { get => _navigationStore.CurrentViewModel; }

	public INavigationViewModel NavigationViewModel { get; }

	public string WindowTitle { get; } = $"StockApp by Daniel Sturm";

	/// <summary>
	/// Zeigt die Versionsnummer vom Assembly
	/// </summary>
	public string VersionNumber { get; } = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
	public string FileName => _turnierStore.FileName == null ? string.Empty : $"Datei: {_turnierStore.FileName}";

	public string ActiveTeamBewerbGroupName => _turnierStore.Turnier?.ContainerTeamBewerbe?.CurrentTeamBewerb?.Gruppenname ?? string.Empty;

	public ICommand NewTournamentCommand { get; init; }
	public ICommand ExitApplicationCommand { get; init; }
	public ICommand OpenTournamentCommand { get; init; }
	public ICommand SaveTournamentCommand { get; init; }
	public ICommand SaveAsTournamentCommand { get; init; }
	public ICommand ClosingCommand { get; init; }

	public ICommand ModalOkCommand { get; set; }
	public ICommand ModalCancelCommand { get; set; }

	private ICommand _setLogLevelCommand;
	public ICommand SetLogLevelCommand => _setLogLevelCommand ??= new RelayCommand(
		(p) =>
		{
			if (p.ToString() == "all")
				ToggleLogLevel(Level.All);
			else if (p.ToString() == "info")
				ToggleLogLevel(Level.Info);
			else if (p.ToString() == "debug")
				ToggleLogLevel(Level.Debug);
			else if (p.ToString() == "off")
				ToggleLogLevel(Level.Off);
			_log.Info($"Changed LogLevel to: {p}");
		},
		(p) => true);

	private ICommand _openLogFileCommand;
	public ICommand OpenLogFileCommand => _openLogFileCommand ??= new RelayCommand(
		(p) =>
		{
			//var rootAppender = ((Hierarchy)LogManager.GetRepository())
			//							 .Root.Appenders.OfType<FileAppender>()
			//							 .FirstOrDefault();

			//string filename = rootAppender != null ? rootAppender.File : string.Empty;
			//string path = Path.GetDirectoryName(filename);
			string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StockApp") + "\\";
			Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
			//todo: Erst prüfen, ob das Verzeichnis existiert. Prüfen, ob das in der StoreApp auch passt
		},
		(p) => true);

	#endregion

	private void ToggleLogLevel(Level level)
	{
		try
		{
			ILoggerRepository[] loggerRepositories = LogManager.GetAllRepositories();
			foreach (var rep in loggerRepositories)
			{
				rep.Threshold = level;
				foreach (ILogger l in ((Hierarchy)rep).GetCurrentLoggers())
				{
					((Logger)l).Level = level;
				}
			}
			Hierarchy h = (Hierarchy)LogManager.GetRepository();
			Logger rootLogger = h.Root;
			rootLogger.Level = level;

			h.RaiseConfigurationChanged(EventArgs.Empty);

			LogLevel = level;
		}
		catch { }
	}
}
