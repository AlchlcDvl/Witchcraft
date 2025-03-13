namespace Witchcraft.Modules;

[Serializable]
public struct HsbColor : IEquatable<HsbColor>
{
    public float h { get; set; }
    public float s { get; set; }
    public float b { get; set; }
    public float a { get; set; }

    public HsbColor(float h, float s, float b, float a = 1f)
    {
        this.h = h;
        this.s = s;
        this.b = b;
        this.a = a;
    }

    public HsbColor(Color col)
    {
        var temp = FromColor(col);
        h = temp.h;
        s = temp.s;
        b = temp.b;
        a = temp.a;
    }

    public override readonly bool Equals(object? obj) => obj is HsbColor color && Equals(color);

    public readonly bool Equals(HsbColor other) => h.Equals(other.h) && s.Equals(other.s) && b.Equals(other.b) && a.Equals(other.a);

    public static bool operator ==(HsbColor left, HsbColor right) => left.Equals(right);

    public static bool operator !=(HsbColor left, HsbColor right) => !(left == right);

    public override readonly int GetHashCode() => HashCode.Combine(h, s, b, a);

    public static HsbColor FromColor(Color color)
    {
        var ret = new HsbColor(0f, 0f, 0f, color.a);

        var r = color.r;
        var g = color.g;
        var b = color.b;

        var max = Mathf.Max(r, Mathf.Max(g, b));

        if (max <= 0)
            return ret;

        var min = Mathf.Min(r, Mathf.Min(g, b));
        var dif = max - min;

        if (max > min)
        {
            if (g == max)
                ret.h = ((b - r) / dif * 60f) + 120f;
            else if (b == max)
                ret.h = ((r - g) / dif * 60f) + 240f;
            else if (b > g)
                ret.h = ((g - b) / dif * 60f) + 360f;
            else
                ret.h = (g - b) / dif * 60f;

            ret.h = ((ret.h % 360f) + 360f) % 360f;
        }
        else
            ret.h = 0;

        ret.h /= 360f;
        ret.s = dif / max;
        ret.b = max;
        return ret;
    }

    public float this[int index]
    {
        readonly get => index switch
        {
            0 => h,
            1 => s,
            2 => b,
            3 => a,
            _ => throw new IndexOutOfRangeException("Invalid HsbColor index (" + index + ")!"),
        };
        set
        {
            switch (index)
            {
                case 0:
                {
                    h = value;
                    break;
                }
                case 1:
                {
                    s = value;
                    break;
                }
                case 2:
                {
                    b = value;
                    break;
                }
                case 3:
                {
                    a = value;
                    break;
                }
                default:
                    throw new IndexOutOfRangeException("Invalid Color index(" + index + ")!");
            }
        }
    }

    public static HsbColor operator +(HsbColor a, HsbColor b) => new(a.h + b.h, a.s + b.s, a.b + b.b, a.a + b.a);

    public static HsbColor operator -(HsbColor a, HsbColor b) => new(a.h - b.h, a.s - b.s, a.b - b.b, a.a - b.a);

    public static HsbColor operator *(HsbColor a, HsbColor b) => new(a.h * b.h, a.s * b.s, a.b * b.b, a.a * b.a);

    public static HsbColor operator /(HsbColor a, HsbColor b) => new(a.h / b.h, a.s / b.s, a.b / b.b, a.a / b.a);

    public static HsbColor operator *(HsbColor a, float b) => new(a.h * b, a.s * b, a.b * b, a.a * b);

    public static HsbColor operator /(HsbColor a, float b) => new(a.h / b, a.s / b, a.b / b, a.a / b);

    public static implicit operator HsbColor(Color col) => new(col);

    public static implicit operator HsbColor(Color32 col) => new(col);

    public static implicit operator Color(HsbColor col) => ToColor(col);

    public static implicit operator Color32(HsbColor col) => ToColor(col);

    public static Color ToColor(HsbColor hsbColor)
    {
        var r = hsbColor.b;
        var g = hsbColor.b;
        var b = hsbColor.b;

        if (hsbColor.s != 0)
        {
            var max = hsbColor.b;
            var dif = hsbColor.b * hsbColor.s;
            var min = hsbColor.b - dif;

            var h = hsbColor.h * 360f;

            switch (h)
            {
                case < 60f:
                {
                    r = max;
                    g = (h * dif / 60f) + min;
                    b = min;
                    break;
                }
                case < 120f:
                {
                    r = (-(h - 120f) * dif / 60f) + min;
                    g = max;
                    b = min;
                    break;
                }
                case < 180f:
                {
                    r = min;
                    g = max;
                    b = ((h - 120f) * dif / 60f) + min;
                    break;
                }
                case < 240f:
                {
                    r = min;
                    g = (-(h - 240f) * dif / 60f) + min;
                    b = max;
                    break;
                }
                case < 300f:
                {
                    r = ((h - 240f) * dif / 60f) + min;
                    g = min;
                    b = max;
                    break;
                }
                case <= 360f:
                {
                    r = max;
                    g = min;
                    b = (-(h - 360f) * dif / 60f) + min;
                    break;
                }
                default:
                {
                    r = 0;
                    g = 0;
                    b = 0;
                    break;
                }
            }
        }

        return new(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), Mathf.Clamp01(hsbColor.a));
    }

    public readonly Color ToColor() => ToColor(this);

    public override readonly string ToString() => $"H: {h} S: {s} B: {b}";

    public static HsbColor Lerp(HsbColor a, HsbColor b, float t)
    {
        float h, s;

        // Check special case black (color.b==0): interpolate neither hue nor saturation!
        // Check special case grey (color.s==0): don't interpolate hue!
        if (a.b == 0)
        {
            h = b.h;
            s = b.s;
        }
        else if (b.b == 0)
        {
            h = a.h;
            s = a.s;
        }
        else
        {
            if (a.s == 0)
                h = b.h;
            else if (b.s == 0)
                h = a.h;
            else
                h = ((Mathf.LerpAngle(a.h * 360f, b.h * 360f, t) % 360f) + 360f) % 360f / 360f;

            s = Mathf.Lerp(a.s, b.s, t);
        }

        return new(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
    }

    public static float PingPong(float min, float max, float mul) => min + Mathf.PingPong(Time.time * mul, max - min);

    public static float PingPongReverse(float min, float max, float mul) => max - Mathf.PingPong(Time.time * mul, max - min);

    // Letting errors be thrown because I want the error makeup to be identical to that of other parse methods
    public static HsbColor Parse(string input)
    {
        input = input.Replace(" ", string.Empty);
        var parts = input.Split(';');

        if (parts.Length is not (3 or 4))
            throw new ArgumentOutOfRangeException(input);

        var parts2 = new List<float>();

        foreach (var part in parts)
        {
            var parts3 = part.Split(',');

            switch (parts3.Length)
            {
                case 1:
                {
                    parts2.Add(float.Parse(parts3[0]));
                    break;
                }
                case 2:
                {
                    parts2.Add(URandom.Range(float.Parse(parts3[0]), float.Parse(parts3[1])));
                    break;
                }
                case 3 or 4:
                {
                    var min = float.Parse(parts3[0]);
                    var max = float.Parse(parts3[1]);
                    var mul = float.Parse(parts3[2]);
                    var reverse = parts.Length == 4 && bool.Parse(parts3[3]);
                    parts2.Add(reverse ? PingPongReverse(min, max, mul) : PingPong(min, max, mul));
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(input);
            }
        }

        return new(parts2[0], parts2[1], parts2[2], parts2.Count == 4 ? parts2[3] : 1f);
    }

    // Since when has try parse ever thrown an error?
    public static bool TryParse(string input, out HsbColor color)
    {
        try
        {
            color = Parse(input);
            return true;
        }
        catch
        {
            color = default;
            return false;
        }
    }
}