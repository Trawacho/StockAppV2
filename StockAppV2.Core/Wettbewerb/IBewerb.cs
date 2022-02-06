using StockApp.Comm.Broadcasting;
using StockApp.Comm.NetMqStockTV;

namespace StockApp.Core.Wettbewerb;

public interface IBewerb
{
    void SetBroadcastData(IBroadCastTelegram telegram);

    void SetStockTVResult(IStockTVResult tVResult);

    void Reset();
}


