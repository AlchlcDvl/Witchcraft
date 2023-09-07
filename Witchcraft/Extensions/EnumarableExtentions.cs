namespace Witchcraft.Extensions;

public static class EnumerableExtentions
{
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action) => source.ToList().ForEach(action);

    public static TValue GetOrCompute<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> supplier) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key))
            return dictionary[key] = supplier();

        return dictionary[key];
    }

    public static void Shuffle<T>(this List<T> list)
    {
        if (list.Count is 1 or 0)
            return;

        for (var i = list.Count - 1; i > 0; --i)
        {
            var r = URandom.RandomRangeInt(0, i + 1);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }

    public static T TakeFirst<T>(this List<T> list)
    {
        try
        {
            var item = list[0];
            list.RemoveAt(0);
            return item;
        }
        catch
        {
            return default;
        }
    }

    public static int RemoveRange<T>(this List<T> list, IEnumerable<T> list2)
    {
        var result = 0;

        foreach (var item in list2)
        {
            if (list.Contains(item))
                result += list.RemoveAll(x => Equals(x, item));
        }

        return result;
    }

    public static void AddRanges<T>(this List<T> main, params IEnumerable<T>[] items) => items.ForEach(main.AddRange);

    public static int RemoveRanges<T>(this List<T> main, params IEnumerable<T>[] items)
    {
        var result = 0;
        items.ForEach(x => result += main.RemoveRange(x));
        return result;
    }

    public static bool Replace<T>(this List<T> list, T item1, T item2, bool all)
    {
        var contains = list.Contains(item1);

        if (contains)
        {
            if (all)
            {
                var pos = 0;
                var clone = list;

                foreach (var item in clone)
                {
                    if (Equals(item, item1))
                    {
                        pos = clone.IndexOf(item);
                        list.Remove(item);
                        list.Insert(pos, item2);
                    }
                }
            }
            else
            {
                var index = list.IndexOf(item1);
                list.Remove(item1);
                list.Insert(index, item2);
            }
        }

        return contains;
    }

    public static T Random<T>(this List<T> list, T defaultVal = default)
    {
        if (list.Count == 0)
            return defaultVal;
        else if (list.Count == 1)
            return list[0];
        else
            return list[URandom.RandomRangeInt(0, list.Count)];
    }

    public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dict, Action<TKey, TValue> action)
    {
        foreach (var pair in dict)
            action(pair.Key, pair.Value);
    }
}