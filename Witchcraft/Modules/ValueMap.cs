using System.Collections;

namespace Witchcraft.Modules;

// Yoinked this lovely piece of code from Daemon at https://github.com/DaemonBeast/Mitochondria/blob/main/Mitochondria.Core/Utilities/Structures/Map.cs albeit with a few changes of my own
public class ValueMap<T1, T2> : IDictionary<T1, T2>, IReadOnlyDictionary<T1, T2> where T1: notnull where T2 : notnull
{
    private readonly Dictionary<T1, T2> Forward;
    private readonly Dictionary<T2, T1> Backward;

    public int Count => Forward.Count;
    public bool IsReadOnly => false;

    public IEnumerable<T1> Keys => Forward.Keys;
    public IEnumerable<T2> Values => Forward.Values;

    ICollection<T1> IDictionary<T1, T2>.Keys => Forward.Keys;
    ICollection<T2> IDictionary<T1, T2>.Values => Forward.Values;

    public ValueMap()
    {
        Forward = [];
        Backward = [];
    }

    public ValueMap(IEnumerable<KeyValuePair<T1, T2>> collection)
    {
        Forward = collection.ToDictionary(p => p.Key, p => p.Value);
        Backward = collection.ToDictionary(p => p.Value, p => p.Key);
    }

    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => Forward.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => Forward.GetEnumerator();

    public void Add(KeyValuePair<T1, T2> item) => Add(item.Key, item.Value);

    public void Clear()
    {
        Forward.Clear();
        Backward.Clear();
    }

    public bool Contains(KeyValuePair<T1, T2> item) => Forward.Contains(item);

    public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) => ((ICollection<KeyValuePair<T1, T2>>)Forward).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<T1, T2> item) => Forward.Remove(item.Key) && Backward.Remove(item.Value);

    public void Add(T1 key, T2 value)
    {
        if (Forward.ContainsKey(key) || Backward.ContainsKey(value))
            throw new ArgumentException("Key or value already in map");

        Forward.Add(key, value);
        Backward.Add(value, key);
    }

    public bool TryAdd(T1 key, T2 value) => Forward.TryAdd(key, value) && Backward.TryAdd(value, key);

    public bool ContainsKey(T1 key) => Forward.ContainsKey(key);

    public bool ContainsValue(T2 value) => Backward.ContainsKey(value);

    public bool Remove(T1 key, out T2 value) => Forward.Remove(key, out value) && Backward.Remove(value);

    public bool Remove(T2 value, out T1 key) => Backward.Remove(value, out key) && Forward.Remove(key);

    public bool Remove(T1 key) => Remove(key, out _);

    public bool Remove(T2 value) => Remove(value, out _);

    public bool TryGetValue(T1 key, out T2 value) => Forward.TryGetValue(key, out value);

    public bool TryGetKey(T2 value, out T1 key) => Backward.TryGetValue(value, out key);

    public T2 this[T1 key]
    {
        get => Forward[key];
        set
        {
            Forward[key] = value;
            Backward[value] = key;
        }
    }

    public T1 this[T2 val]
    {
        get => Backward[val];
        set
        {
            Forward[value] = val;
            Backward[val] = value;
        }
    }
}