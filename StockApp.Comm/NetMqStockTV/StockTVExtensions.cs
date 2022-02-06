using System.Collections.ObjectModel;

namespace StockApp.Comm.NetMqStockTV;

static class StockTVExtensions
{
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> array, int size)
    {
        for (var i = 0; i < (float)array.Count() / size; i++)
        {
            yield return array.Skip(i * size).Take(size);
        }
    }

    public static void Sort<T>(this ObservableCollection<T> collection, bool asc)
           where T : IComparable<T>, IEquatable<T>
    {
        List<T> sorted = asc ? collection.OrderBy(x => x).ToList() : collection.OrderByDescending(x => x).ToList();

        int ptr = 0;
        while (ptr < sorted.Count - 1)
        {
            if (!collection[ptr].Equals(sorted[ptr]))
            {
                int idx = Search(collection, ptr + 1, sorted[ptr]);
                collection.Move(idx, ptr);
            }

            ptr++;
        }
    }

    public static int Search<T>(Collection<T> collection, int startIndex, T other)
    {
        for (int i = startIndex; i < collection.Count; i++)
        {
            if (other.Equals(collection[i]))
                return i;
        }

        return -1; // decide how to handle error case


    }

}
