namespace Witchcraft.Utils;

/// <summary>A class for handling data and elements of <see cref="IEnumerable{T}"/> and <see cref="Dictionary{T, T}"/>.</summary>
public static class EnumerableUtils
{
    /// <summary>Performs the specified action on each element of the <see cref="List{T}"/>.</summary>
    /// <param name="source">The <see cref="IEnumerable{T}"/> whose elements have <paramref name="action"/> performed on them.</param>
    /// <param name="action">The <see cref="Action{T}"/> that needs to be performed on every element in <paramref name="source"/>.</param>
    /// <typeparam name="T">The element type of <paramref name="source"/>.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> was <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Operation was not possible.</exception>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action) => source.ToList().ForEach(action);

    /// <summary>Gets the value associated with the specified key. If it fails, it will return the supplied value instead while inserting it into the dictionary.</summary>
    /// <param name="dict">The <see cref="Dictionary{T, T}"/> from which a value is being obtainted from.</param>
    /// <param name="key">The <typeparamref name="TKey"/> tassociated with the value attempting to be retrieved.</param>
    /// <param name="supplier">The <see cref="Func{TValue}"/> that acts as a replacement should the key not exist.</param>
    /// <typeparam name="TKey">The key.</typeparam>
    /// <typeparam name="TValue">The value.</typeparam>
    /// <returns>The value associated with the specified key. If it fails, it will return the supplied value instead and adds the failed key and new value into the dictionary.</returns>
    public static TValue? GetOrCompute<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> supplier)
        where TKey : notnull
    {
        if (!dict.ContainsKey(key))
            return dict[key] = supplier();

        return dict[key];
    }

    /// <summary>Shuffles the elements within <paramref name="list"/>.</summary>
    /// <param name="list">The <see cref="List{T}"/> that needs to be shuffled.</param>
    /// <typeparam name="T">The element type of <paramref name="list"/>.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
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

    /// <summary>Finds the first element in <paramref name="list"/>.</summary>
    /// <param name="list">The <see cref="List{T}"/> whose first element must be taken.</param>
    /// <typeparam name="T">The element type of <paramref name="list"/>.</typeparam>
    /// <returns>The first element of <paramref name="list"/> of type <typeparamref name="T"/>.</returns>
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

    /// <summary>Removes every element in <paramref name="list2"/> from <paramref name="list"/>.</summary>
    /// <param name="list">The <see cref="List{T}"/> that needs to have the elements from <paramref name="list2"/> removed.</param>
    /// <param name="list2">The <see cref="IEnumerable{T}"/> whose elements need to be removed from <paramref name="list"/>.</param>
    /// <typeparam name="T">The element type of <paramref name="list"/>.</typeparam>
    /// <returns>The number of elements that were removed as a result.</returns>
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

    /// <summary>Adds every element from the <see cref="List{T}"/>s in <paramref name="items"/> into <paramref name="main"/>.</summary>
    /// <param name="main">The <see cref="List{T}"/> to have elements added to it.</param>
    /// <param name="items">The <see cref="IEnumerable{T}"/>s whose elements need to be added to <paramref name="main"/>.</param>
    /// <typeparam name="T">The element type of <paramref name="main"/>.</typeparam>
    public static void AddRanges<T>(this List<T> main, params IEnumerable<T>[] items) => items.ForEach(main.AddRange);

    /// <summary>Removes every element in <see cref="List{T}"/>s in <paramref name="items"/> from <paramref name="main"/>.</summary>
    /// <param name="main">The <see cref="List{T}"/> to have elements removed from it.</param>
    /// <param name="items">The <see cref="IEnumerable{T}"/>s whose elements need to be removed from <paramref name="main"/>.</param>
    /// <typeparam name="T">The element type of <paramref name="main"/>.</typeparam>
    /// <returns>The number of elements that were removed as a result.</returns>
    public static int RemoveRanges<T>(this List<T> main, params IEnumerable<T>[] items)
    {
        var result = 0;
        items.ForEach(x => result += main.RemoveRange(x));
        return result;
    }

    /// <summary>Replaces the first instance of <paramref name="item1"/> in <paramref name="list"/> with <paramref name="item2"/>.</summary>
    /// <param name="list">The <see cref="List{T}"/> that needs to be shuffled.</param>
    /// <param name="item1">The element that needs to be replaced.</param>
    /// <param name="item2">The element that <paramref name="item1"/> needs to be replaced with.</param>
    /// <param name="all">Decides whether all or the first instance of <paramref name="item1"/> needs to be replaced.</param>
    /// <typeparam name="T">The element type of <paramref name="list"/>.</typeparam>
    /// <returns><see langword="false"/> if the replace was unsuccessful or if <paramref name="list"/> did not contain <paramref name="item1"/>, otherwise <see langword="true"/>.</returns>
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

    /// <summary>Finds a random element from <paramref name="input"/>.</summary>
    /// <param name="input">The <see cref="List{T}"/> to find a random element from.</param>
    /// <param name="defaultVal">The <see langword="default"/> value to return if the method fails.</param>
    /// <typeparam name="T">The element type of <paramref name="input"/>.</typeparam>
    /// <returns>A random element from <paramref name="input"/>.</returns>
    public static T? Random<T>(this IEnumerable<T> input, T defaultVal = default!)
    {
        var list = (input as IList<T>) ?? input.ToList();

        if (list.Count != 0)
            return list[URandom.Range(0, list.Count)];

        return defaultVal;
    }

    /// <summary>Finds a random element from <paramref name="input"/>.</summary>
    /// <param name="input">The <see cref="List{T}"/> to find a random element from.</param>
    /// <param name="predicate">The condition that the random element should fulfil.</param>
    /// <param name="defaultVal">The <see langword="default"/> value to return if the method fails.</param>
    /// <typeparam name="T">The element type of <paramref name="input"/>.</typeparam>
    /// <returns>A random element from <paramref name="input"/>.</returns>
    public static T? Random<T>(this IEnumerable<T> input, Func<T, bool> predicate, T defaultVal = default!) => input.Where(predicate).Random(defaultVal);

    /// <summary>Performs the specified action on each element of the <see cref="IDictionary{T, T}"/>.</summary>
    /// <param name="dict">The <see cref="IDictionary{T, T}"/> whose elements have <paramref name="action"/> performed on them.</param>
    /// <param name="action">The <see cref="Action{T}"/> that needs to be performed on every element in <paramref name="dict"/>.</param>
    /// <typeparam name="TKey">The key.</typeparam>
    /// <typeparam name="TValue">The value.</typeparam>
    /// <exception cref="ArgumentNullException"><paramref name="dict"/> was <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Operation was not possible.</exception>
    public static void ForEach<TKey, TValue>(this IDictionary<TKey, TValue> dict, Action<TKey, TValue> action) => dict.ToList().ForEach(pair => action(pair.Key, pair.Value));
}
