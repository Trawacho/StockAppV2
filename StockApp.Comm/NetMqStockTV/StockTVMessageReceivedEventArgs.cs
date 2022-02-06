namespace StockApp.Comm.NetMqStockTV;

public class StockTVMessageReceivedEventArgs : EventArgs
{
    public MessageTopic MessageTopic { get; set; }
    public byte[] MessageValue { get; set; }
    public StockTVMessageReceivedEventArgs()
    {

    }
    public StockTVMessageReceivedEventArgs(MessageTopic topic, byte[] value) : this()
    {
        MessageTopic = topic;
        MessageValue = value;
    }
}

