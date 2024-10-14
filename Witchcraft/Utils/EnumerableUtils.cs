namespace Witchcraft.Utils;

public static class EnumerableUtils
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dict, Action<TKey, TValue> action) => dict.ToList().ForEach(pair => action(pair.Key, pair.Value));

    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var num = 0;
        var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
            action(enumerator.Current, num++);

        enumerator.Dispose();
    }

    public static TValue? GetOrCompute<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> supplier) where TKey : notnull
    {
        if (!dict.TryGetValue(key, out var value))
            return dict[key] = supplier();

        return value;
    }

    public static void Shuffle<T>(this List<T> list)
    {
        if (list.Count is 1 or 0)
            throw new NotImplementedException(nameof(list));

        for (var i = list.Count - 1; i > 0; --i)
        {
            var r = URandom.RandomRangeInt(0, i + 1);
            (list[r], list[i]) = (list[i], list[r]);
        }
    }

    public static T? TakeFirst<T>(this List<T> list)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));

        if (list.Count == 0)
            throw new NotImplementedException(nameof(list));

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

        try
        {
            if (contains)
            {
                if (all)
                {
                    var clone = new List<T>(list);

                    foreach (var item in clone)
                    {
                        if (Equals(item, item1))
                        {
                            var pos = clone.IndexOf(item);

                            if (list.Remove(item))
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
        }
        catch
        {
            contains = false;
        }

        return contains;
    }

    public static T? Random<T>(this IEnumerable<T> input, T? defaultVal = default)
    {
        var list = input as IList<T> ?? input.ToList();
        return list.Count == 0 ? defaultVal : list[URandom.Range(0, list.Count)];
    }

    public static T? Random<T>(this IEnumerable<T> input, Func<T, bool> predicate, T? defaultVal = default) => input.Where(predicate).Random(defaultVal);

    public static T? Random<T>(this IEnumerable<T> input, SRandom random, T? defaultVal = default)
    {
        var list = input as IList<T> ?? input.ToList();
        return list.Count == 0 ? defaultVal : list[random.Next(0, list.Count)];
    }

    public static List<List<T>> Split<T>(this List<T> list, int splitCount)
    {
        var result = new List<List<T>>();
        var temp = new List<T>();

        foreach (var item in list)
        {
            temp.Add(item);

            if (temp.Count == splitCount)
            {
                result.Add(temp);
                temp = [];
            }
        }

        if (temp.Count > 0)
            result.Add(temp);

        return result;
    }

    public static List<List<T>> Split<T>(this List<T> list, Func<T, bool> splitCondition, bool includeSatisfier = true)
    {
        var result = new List<List<T>>();
        var temp = new List<T>();

        foreach (var item in list)
        {
            if (splitCondition(item))
            {
                if (includeSatisfier)
                    temp.Add(item);

                result.Add(temp);
                temp = [];
            }
            else
                temp.Add(item);
        }

        if (temp.Count > 0)
            result.Add(temp);

        return result;
    }

    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        for (var i = 0; i < source.Count(); i++)
        {
            if (predicate(source.ElementAt(i)))
                return i;
        }

        return -1;
    }

    public static List<T> GetRandomRange<T>(this IEnumerable<T> input, int count, T? defaultVal = default)
    {
        var temp = new List<T>();

        if (input.Count() <= count)
            temp.AddRange(input);
        else
        {
            while (temp.Count < count)
                temp.Add(input.Random(x => !temp.Contains(x), defaultVal)!);
        }

        temp.Shuffle();
        return temp;
    }

    public static bool TryFinding<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T? value)
    {
        try
        {
            value = source.First(predicate);
            return true;
        }
        catch
        {
            value = default;
            return false;
        }
    }

    public static void AddMany<T>(this IEnumerable<T> list, T item, int count)
    {
        while (count > 0)
        {
            list = list.AddItem(item);
            count--;
        }
    }

    public static bool AllAnyOrEmpty<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool all = false) => !source.Any() || (all ? source.All(predicate) : source.Any(predicate));
}