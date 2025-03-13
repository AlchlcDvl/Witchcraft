namespace Witchcraft.Utils;

public static class NumberUtils
{
    public static bool IsInRange(this float num, float min, float max, bool minInclusive = false, bool maxInclusive = false)
    {
        switch (minInclusive)
        {
            case true when maxInclusive:
                return num >= min && num <= max;
            case true:
                return num >= min && num < max;
        }

        return num > min && (maxInclusive ? num <= max : num < max);
    }

    public static bool IsInRange(this int num, float min, float max, bool minInclusive = false, bool maxInclusive = false) => ((float)num).IsInRange(min, max, minInclusive, maxInclusive);

    public static bool Check(int probability) => probability switch
    {
        0 => false,
        100 => true,
        _ => URandom.RandomRangeInt(1, 100) <= probability
    };

    public static float CycleFloat(float max, float min, float currentVal, bool increment, float change = 1f)
    {
        var value = change * (increment ? 1 : -1);
        currentVal += value;

        if (currentVal > max)
            currentVal = min;
        else if (currentVal < min)
            currentVal = max;

        return currentVal;
    }

    public static int CycleInt(int max, int min, int currentVal, bool increment, int change = 1) => (int)CycleFloat(max, min, currentVal, increment, change);

    public static byte CycleByte(int max, int min, int currentVal, bool increment, int change = 1) => (byte)CycleInt(max, min, currentVal, increment, change);

    public static bool[] ToBits(this byte @byte)
    {
        var result = new bool[8];

        for (var i = 0; i < 8; i++)
            result[i] = @byte.ToBit(i);

        return result;
    }

    public static bool ToBit(this byte @byte, int index) => (@byte & (1 << index)) != 0;

    public static byte ToByte(this bool[] bits)
    {
        byte result = 0;

        for (var i = 0; i < bits.Length; i++)
            result |= bits[i].ToByte(i);

        return result;
    }

    public static byte ToByte(this bool bit, int index) => (byte)(bit ? (1 << index) : 0);

    public static float ZigZag(float mul, float length)
    {
        var dx = mul * Time.time;
        var f = Mathf.FloorToInt(dx);
        var m = f % 2;
        return length * (((dx - f) * ((2 * m) - 1)) + 1 - m);
    }
}