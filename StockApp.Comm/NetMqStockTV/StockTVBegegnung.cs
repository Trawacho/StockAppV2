namespace StockApp.Comm.NetMqStockTV
{
    public class StockTVBegegnung
    {
        public StockTVBegegnung()
        {

        }
        public StockTVBegegnung(int spielNummer, string teamNameA, string teamNameB, bool isAnspielTeamA) : this()
        {
            SpielNummer = spielNummer;
            TeamNameA = teamNameA;
            TeamNameB = teamNameB;
            IsAnspielTeamA = isAnspielTeamA;
        }
        public int SpielNummer { get; set; }
        public string TeamNameA { get; set; }
        public string TeamNameB { get; set; }
        public bool IsAnspielTeamA { get; set; }

        public string GetStockTVString(bool nextCourtLeft)
        {
            if (IsAnspielTeamA && nextCourtLeft)
                return $"{SpielNummer}:{TeamNameA} »:{TeamNameB};";
            else if (!IsAnspielTeamA && nextCourtLeft)
                return $"{SpielNummer}:{TeamNameA}:« {TeamNameB};";


            else if (IsAnspielTeamA && !nextCourtLeft)
                return $"{SpielNummer}:« {TeamNameA}:{TeamNameB};";
            else if(!IsAnspielTeamA && !nextCourtLeft)
                return $"{SpielNummer}:{TeamNameA}:{TeamNameB} »;";

            else
                return $"{SpielNummer}:{TeamNameA}:{TeamNameB};";

        }
    }
}
