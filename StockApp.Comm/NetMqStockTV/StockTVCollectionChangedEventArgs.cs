namespace StockApp.Comm.NetMqStockTV
{
    public class StockTVCollectionChangedEventArgs : EventArgs
    {
        public StockTVCollectionChangedEventArgs(bool added)
        {
            this.Added = added;
        }

        public bool Added { get; }
        public bool Removed => !Added;
    }
}
