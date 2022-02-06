namespace StockApp.Comm.Broadcasting
{
    public interface IBroadCastTelegram : IEquatable<IBroadCastTelegram>
    {
        byte StockTVModus { get; }
        byte Spielrichtung { get; }
        byte BahnNummer { get; }
        byte[] Values { get; }
        byte SpielGruppe { get; }

        byte[] Telegram { get; }

        IBroadCastTelegram Copy();

    }

    public class BroadCastTelegram : IBroadCastTelegram
    {
        private readonly byte[] _telegram;
        private readonly byte[] _values;

        public BroadCastTelegram(byte[] telegram)
        {
            this._telegram = telegram;

            _values = new byte[telegram.Length - 10];
            Array.Copy(telegram, 10, _values, 0, telegram.Length - 10);

        }

        public byte[] Telegram => _telegram;
        public byte[] Values => _values;

        public byte BahnNummer => _telegram[0];

        public byte SpielGruppe => _telegram[1];

        /// <summary>
        /// Wert kommt aus StockTV GameSettings.GameModis
        /// <br>0 = Training</br>
        /// <br>1 = BestOf</br>
        /// <br>2 = Turnier</br>
        /// <br>100 = Ziel</br>
        /// </summary>
        public byte StockTVModus => _telegram[2];

        /// <summary>
        /// <para>Wert aus StockTV ColorScheme.NextBahnModis</para>
        /// <br>0 = Links</br>
        /// <br>1 = Rechts</br>
        /// </summary>
        public byte Spielrichtung => _telegram[3];

        public bool Equals(IBroadCastTelegram other)
        {
            if (other == null) return false;
            return _telegram.SequenceEqual(other.Telegram);
        }

        public IBroadCastTelegram Copy()
        {
            byte[] newTelegram = new byte[_telegram.Length];
            Array.Copy(_telegram, 0, newTelegram, 0, _telegram.Length);
            return new BroadCastTelegram(newTelegram);
        }

    }
}
