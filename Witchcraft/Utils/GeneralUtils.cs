using System.Collections;
using System.Diagnostics;

namespace Witchcraft.Utils;

public static class GeneralUtils
{
    public static int GetNumeral(this BitArray array, int startIndex, int bitLength)
    {
        var newArray = new BitArray(bitLength);

        for (var i = 0; i < bitLength; i++)
            newArray[i] = array.Length > startIndex + i && array.Get(startIndex + i);

        return newArray.ToNumeral();
    }

    public static int ToNumeral(this BitArray? array)
    {
        if (array == null)
        {
            Witchcraft.Instance!.Error("Array is nothing.");
            return 0;
        }

        if (array.Length > 32)
        {
            Witchcraft.Instance!.Error("Must be at most 32 bits long.");
            return 0;
        }

        var result = new int[1];
        array.CopyTo(result, 0);
        return result[0];
    }

    public static MethodInfo? GetMethod(this Type type, Func<MethodInfo, bool> predicate) => type.GetMethods(AccessTools.all).Find(predicate);

    public static T[]? GetEnumValues<T>() where T : struct, Enum => Enum.GetValues(typeof(T)) as T[];

    public static bool IsAny<T>(this T item, params T[] items) => items.Contains(item);

    public static bool HasAnyFlag<T>(this T value, params T[] flags) where T : struct, Enum
    {
        var tType = typeof(T);

        if (tType.GetCustomAttribute<FlagsAttribute>() == null)
            throw new ArgumentException($"{tType.Name} is not a flag enum");

        return flags.Any(x => value.HasFlag(x));
    }
}