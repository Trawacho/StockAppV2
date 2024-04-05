using System;

namespace StockApp.UI.Settings;

public interface ILogger
{
    void DebugFormat(string a, string b);
    void Error(string error);
    void Error(Exception e);
    void Info(string message);
    void InfoFormat(string a, string b);
    void InfoFormat(string a, string b, string c);
    void InfoFormat(string a, string b, string c, string d);
    void InfoFormat(string a, DateTime dateTime);

}
