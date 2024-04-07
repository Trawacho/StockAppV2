using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;
using StockApp.UI.com;
using StockApp.UI.Services;
using StockApp.UI.Settings;
using StockApp.UI.Stores;
using StockApp.UI.ViewModels;
using StockApp.UI.Views;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace StockApp.UI
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly INavigationStore _navigationStore;
        private readonly ITurnierStore _turnierStore;
        private readonly IDialogStore _dialogStore;
        private readonly INavigationViewModel _navigationViewModel;
        private readonly IStockTVService _stockTVService;
        private readonly IStockTVCommandStore _stockTVCommandStore;
        private readonly ITurnierNetworkManager _turnierNetworkManager;
        private readonly IBroadcastService _broadCastService;
        private readonly MainViewModel _mainViewModel;
        private readonly MainWindow _mainWindow;
        private readonly string _assemblyVersion;

        public App()
        {
            _dialogStore = new DialogStore(null);
            _dialogStore.Register<LiveResultsTeamViewModel, LiveResultTeamView>();
            _dialogStore.Register<LiveResultsZielViewModel, LiveResultZielView>();

            var _systemVersion = Assembly.GetExecutingAssembly().GetName().Version;
            _assemblyVersion = _systemVersion.ToString();
            Software.Initialize(_systemVersion);
            Software.SanityCheckDirectories();
            PreferencesManager.Initialize();
            Software.ConfigureInstance();


            _navigationStore = new NavigationStore();


            var _templateVereine = VereineFactory.Load();
            _turnierStore = new TurnierStore(_templateVereine);

            _stockTVService = new StockTVService();
            _broadCastService = new BroadcastService();

            _stockTVCommandStore = new StockTVCommandStore(_stockTVService);

            _turnierNetworkManager = new TurnierNetworkManager(_turnierStore, _stockTVService, _broadCastService);

            _navigationViewModel = new NavigationViewModel(turnierStore: _turnierStore,
                                                           teamBewerbContainerNavigationService: CreateTeamBewerbContainerNavigationService(),
                                                           contestNavigationService: CreateContestNavigationService(),
                                                           turnierNavigationService: CreateTurnierNavigationService(),
                                                           teamsNavigationService: CreateTeamsNavigationService(),
                                                           gamesNavigationService: CreateGamesNavigationService(),
                                                           resultsNavigationService: CreateResultsNavigationService(),
                                                           stockTVsNavigationService: CreateStockTVsNavigationService(),
                                                           zielTeilnehmerNavigationService: CreateZielTeilnehmerNavigationService(),
                                                           zielDruckNavigationService: CreateZielDruckNavigationService(),
                                                           outputNavigationService: CreateOutputNavigationService(),
                                                           liveResultZielDialogService: CreateLiveReusltsZielDialogService());



            _mainViewModel = new MainViewModel(_navigationViewModel, _navigationStore, _turnierStore);
            _mainWindow = new MainWindow() { DataContext = _mainViewModel };

            PreferencesManager.GeneralAppSettings.WindowPlaceManager.Register(_mainWindow, "MainWindow");
        }



        protected override void OnStartup(StartupEventArgs e)
        {
            for (int i = 0; i != e.Args.Length; ++i)
            {
                if (e.Args[i].EndsWith(".skmr"))
                    _turnierStore.Load(e.Args[i]);
            }

            INavigationService<TurnierViewModel> turnierNavigationService = CreateTurnierNavigationService();
            turnierNavigationService.Navigate();
            _stockTVService.Discover();

            _mainViewModel.RequestClose += RequestCloseHandler;
            MainWindow = _mainWindow;

            _dialogStore.SetOwner(MainWindow);
            _mainWindow.Show();

            base.OnStartup(e);
        }

        private void RequestCloseHandler(object sender, CancelEventArgs e)
        {
            _mainViewModel.RequestClose -= RequestCloseHandler;
            App.Current.Shutdown();
        }

        #region CreateDialogServices
        private IDialogService<LiveResultsZielViewModel> CreateLiveReusltsZielDialogService()
        {
            return new DialogService<LiveResultsZielViewModel>(_dialogStore, () => new LiveResultsZielViewModel(_turnierStore), false);
        }
        #endregion

        #region CreateNavigationServices

        private INavigationService<TurnierViewModel> CreateTurnierNavigationService()
        {
            return new NavigationService<TurnierViewModel>(_navigationStore, () => new TurnierViewModel(_turnierStore));
        }

        private INavigationService<WettbewerbsartViewModel> CreateContestNavigationService()
        {
            return new NavigationService<WettbewerbsartViewModel>(_navigationStore, () => new WettbewerbsartViewModel(_turnierStore));
        }

        private INavigationService<TeamBewerbContainerViewModel> CreateTeamBewerbContainerNavigationService()
        {
            return new NavigationService<TeamBewerbContainerViewModel>(_navigationStore, () => new TeamBewerbContainerViewModel(_turnierStore));
        }

        private INavigationService<StockTVCollectionViewModel> CreateStockTVsNavigationService()
        {
            return new NavigationService<StockTVCollectionViewModel>(_navigationStore, () => new StockTVCollectionViewModel(_stockTVService, _stockTVCommandStore, _turnierStore));
        }

        private INavigationService<ResultsViewModel> CreateResultsNavigationService()
        {
            return new NavigationService<ResultsViewModel>(_navigationStore, () => new ResultsViewModel(_turnierStore, _dialogStore, _turnierNetworkManager));
        }

        private INavigationService<GamesViewModel> CreateGamesNavigationService()
        {
            return new NavigationService<GamesViewModel>(_navigationStore, () => new GamesViewModel(_turnierStore));
        }

        private INavigationService<TeamsViewModel> CreateTeamsNavigationService()
        {
            return new NavigationService<TeamsViewModel>(_navigationStore, () => new TeamsViewModel(_turnierStore));
        }

        private INavigationService<ZielBewerbViewModel> CreateZielTeilnehmerNavigationService()
        {
            return new NavigationService<ZielBewerbViewModel>(_navigationStore, () => new ZielBewerbViewModel(_turnierStore, _turnierNetworkManager));
        }

        private INavigationService<ZielBewerbDruckViewModel> CreateZielDruckNavigationService()
        {
            return new NavigationService<ZielBewerbDruckViewModel>(_navigationStore, () => new ZielBewerbDruckViewModel(_turnierStore));
        }

        private INavigationService<OptionsViewModel> CreateOutputNavigationService()
        {
            return new NavigationService<OptionsViewModel>(_navigationStore, ()=> new OptionsViewModel(_turnierStore));
        }

        #endregion





        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                _turnierNetworkManager?.Dispose();
                _stockTVService?.Dispose();
                _broadCastService?.Dispose();
                PreferencesManager.Save();
            }
            finally
            {
                _navigationViewModel.Dispose();

                base.OnExit(e);
            }
            Application.Current.Shutdown(0);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

        }
    }
}
