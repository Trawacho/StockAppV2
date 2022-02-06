
namespace StockApp.Core.Wettbewerb.Zielbewerb
{
    public interface IWertung
    {
        public int Nummer { get; set; }
        public IEnumerable<IDisziplin> Disziplinen { get; }
        public int GesamtPunkte { get; }
        public bool IsOnline { get; set; }
        bool VersucheAllEntered();
        void Reset();
        int PunkteMassenMitte { get; }
        int PunkteSchuesse { get; }
        int PunkteMassenSeitlich { get; }
        int PunkteKombinieren { get; }

        event EventHandler OnlineStatusChanged;
    }

    internal class Wertung : IWertung
    {
        private readonly List<IDisziplin> _disziplinen;
        private bool _isOnline;

        public event EventHandler OnlineStatusChanged;
        protected void RaiseOnlineStatusChanged()
        {
            var handler = OnlineStatusChanged;
            handler?.Invoke(this, EventArgs.Empty);
        }
        /// <summary>
        /// Default-Konstruktor
        /// </summary>
        private Wertung()
        {
            var massenMitte = Disziplin.Create(Disziplinart.MassenMitte);

            var schiessen = Disziplin.Create(Disziplinart.Schiessen);

            var massenSeite = Disziplin.Create(Disziplinart.MassenSeite);

            var kombinieren = Disziplin.Create(Disziplinart.Komibinieren);

            _disziplinen = new List<IDisziplin> { massenMitte, schiessen, massenSeite, kombinieren };

            _isOnline = false;
        }

        public static IWertung Create(int numberOfWertung) => new Wertung() { Nummer = numberOfWertung };

        /// <summary>
        /// Nummer der Wertung
        /// </summary>
        public int Nummer { get; set; }

        /// <summary>
        /// Sammlung der 4 Disziplinen
        /// </summary>
        public IEnumerable<IDisziplin> Disziplinen { get => _disziplinen; }


        /// <summary>
        /// TRUE wenn diese Wertung vom Teilnehmer online ist
        /// </summary>
        public bool IsOnline { get => _isOnline; set { _isOnline = value; RaiseOnlineStatusChanged(); } }

        /// <summary>
        /// Jeder Versuch in jeder Disziplin wird auf -1 gesetzt
        /// </summary>
        public void Reset()
        {
            foreach (var disziplin in _disziplinen)
            {
                disziplin.Reset();
            }
        }

        /// <summary>
        /// Anzahl der Versuche die eingegeben wurden ( 0 - 24 )
        /// </summary>
        /// <returns></returns>
        internal int VersucheCount() => Disziplinen.Sum(d => d.VersucheCount());

        /// <summary>
        /// True, wenn alle Versuche eingegeben wurdne (24 Veruche)
        /// </summary>
        /// <returns></returns>
        public bool VersucheAllEntered() => VersucheCount() == 24;

        //internal bool AddVersuch(int value)
        //{
        //    foreach (var d in _disziplinen)
        //    {
        //        if (d.AddVersuch(value))
        //        {
        //            return true;
        //        }
        //    }

        //    return false;
        //}

        #region READONLY  Punkte

        public int GesamtPunkte => Disziplinen.Sum(d => d.Summe);

        public int PunkteMassenMitte => Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Summe;

        public int PunkteSchuesse => Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Summe;

        public int PunkteMassenSeitlich => Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenSeite).Summe;

        public int PunkteKombinieren => Disziplinen.First(d => d.Disziplinart == Disziplinart.Komibinieren).Summe;

        #endregion



    }
}
