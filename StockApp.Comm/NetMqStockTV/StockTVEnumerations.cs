namespace StockApp.Comm.NetMqStockTV
{
    /// <summary>
    /// Training = 0 <br/>
    /// BestOf   = 1   <br/>
    /// Turnier  = 2  <br/>
    /// Ziel     = 100   <br/>
    /// </summary>
    public enum GameMode
    {
        Training = 0,
        Turnier = 2,
        BestOf = 1,
        Ziel = 100
    }

    /// <summary>
    /// Normal = 0  <br/>
    /// Dark   = 1
    /// </summary>
    public enum ColorMode
    {
        Normal = 0,
        Dark = 1
    }

    /// <summary>
    /// Left = 0, <br/>
    /// Right = 1
    /// </summary>
    public enum NextCourtMode
    {
        Left = 0,
        Right = 1
    }

    public enum MessageTopic
    {
        Hello = 01,
        Welcome = 02,
        Alive = 03,
        SetResult = 10,
        GetResult = 11,
        SetSettings = 20,
        GetSettings = 21,
        ResetResult = 30,
        SetTeamNames = 40,
        SetTeilnehmer = 41,
        SetImage = 50,
        GoToImage = 51,
        ClearImage = 52
    }
}
