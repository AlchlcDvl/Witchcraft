namespace Witchcraft.Extensions;

public static class EnumerableExtentions
{
    public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        if (action == null)
            throw new ArgumentNullException(nameof(action));

        var enumerator = source.GetEnumerator();

        while (enumerator.MoveNext())
            action(enumerator.Current);

        enumerator.Dispose();
    }
}