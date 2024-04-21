
namespace StockApp.Core.Wettbewerb.Zielbewerb
{
    public interface IWertung
    {
        /// <summary>
        /// Fortlaufende Nummer der Wertung
        /// </summary>
        public int Nummer { get; set; }
        /// <summary>
        /// Disziplinen der Wertung
        /// </summary>
        public IEnumerable<IDisziplin> Disziplinen { get; }
        /// <summary>
        /// Summer aller Punkte
        /// </summary>
        public int GesamtPunkte { get; }
        /// <summary>
        /// True, wenn diese Wertung durch StockTV Daten gefüllt wird
        /// </summary>
        public bool IsOnline { get; set; }
        /// <summary>
        /// True, wenn alle 24 Ergebnisse in der Wertung stehen
        /// </summary>
        /// <returns></returns>
        bool VersucheAllEntered();
        /// <summary>
        /// Es werden alle Werte der Versuche auf -1 gesetzt
        /// </summary>
        void Reset();
        /// <summary>
        /// Summe der Punkte für die <see cref="Disziplinart.MassenMitte"/>
        /// </summary>
        int PunkteMassenMitte { get; }
        /// <summary>
        /// Summe der Punkte für die <see cref="Disziplinart.Schiessen"/>
        /// </summary>
        int PunkteSchuesse { get; }
        /// <summary>
        /// Summe der Punkte für die <see cref="Disziplinart.MassenSeite"/>
        /// </summary>
        int PunkteMassenSeitlich { get; }
        /// <summary>
        /// Summe der Punkte für die <see cref="Disziplinart.Kombinieren"/>
        /// </summary>
        int PunkteKombinieren { get; }

        /// <summary>
        /// Wird ausgeführt, wenn sich <see cref="IsOnline"/> ändert
        /// </summary>

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

            var kombinieren = Disziplin.Create(Disziplinart.Kombinieren);

            _disziplinen = new List<IDisziplin> { massenMitte, schiessen, massenSeite, kombinieren };

            _isOnline = false;
        }

        public static IWertung Create(int numberOfWertung) => new Wertung() { Nummer = numberOfWertung };

        public int Nummer { get; set; }


        public IEnumerable<IDisziplin> Disziplinen { get => _disziplinen; }



        public bool IsOnline { get => _isOnline; set { _isOnline = value; RaiseOnlineStatusChanged(); } }


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


        public bool VersucheAllEntered() => VersucheCount() == 24;

        #region READONLY  Punkte

        public int GesamtPunkte => Disziplinen.Sum(d => d.Summe);

        public int PunkteMassenMitte => Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenMitte).Summe;

        public int PunkteSchuesse => Disziplinen.First(d => d.Disziplinart == Disziplinart.Schiessen).Summe;

        public int PunkteMassenSeitlich => Disziplinen.First(d => d.Disziplinart == Disziplinart.MassenSeite).Summe;

        public int PunkteKombinieren => Disziplinen.First(d => d.Disziplinart == Disziplinart.Kombinieren).Summe;

        #endregion



    }
}
