namespace StockApp.UI.Extensions;

internal static class IntExtensions
{
    internal static int InRange(this int value, int min, int max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
