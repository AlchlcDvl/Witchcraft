namespace Witchcraft.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrCompute<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue supplier) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key))
            return dictionary[key] = supplier;

        return dictionary[key];
    }
}