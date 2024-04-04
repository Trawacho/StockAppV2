namespace StockApp.Lib.Extensions;
public static class IntExtensions
{
    public static int InRange(this int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}