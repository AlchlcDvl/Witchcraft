namespace Witchcraft.Modules;

/// <summary>A color mechanic based on the HSB color system.</summary>
[Serializable]
public struct HSBColor
{
    /// <summary>Hue.</summary>
    public float h;

    /// <summary>Saturation.</summary>
    public float s;

    /// <summary>Brightness.</summary>
    public float b;

    /// <summary>Transparancy.</summary>
    public float a;

    /// <summary>Initializes a new instance of the <see cref="HSBColor"/> struct.</summary>
    /// <param name="h">Dictates the color that it appears as.</param>
    /// <param name="s">How grey must the color be.</param>
    /// <param name="b">How bright must the color be.</param>
    /// <param name="a">How opaque the color is.</param>
    /// <returns>An <see cref="HSBColor"/> using the given values.</returns>
    public HSBColor(float h, float s, float b, float a)
    {
        this.h = h;
        this.s = s;
        this.b = b;
        this.a = a;
    }

    /// <summary>Initializes a new instance of the <see cref="HSBColor"/> struct.</summary>
    /// <param name="h">Dictates the color that it appears as.</param>
    /// <param name="s">How grey must the color be.</param>
    /// <param name="b">How bright must the color be.</param>
    /// <returns>An <see cref="HSBColor"/> using the given values.</returns>
    public HSBColor(float h, float s, float b)
        : this(h, s, b, 1f)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="HSBColor"/> struct.</summary>
    /// <param name="col">Dictates the color that it appears as.</param>
    /// <returns>An <see cref="HSBColor"/> corresponding to the given <see cref="Color"/>.</returns>
    public HSBColor(Color col)
    {
        var temp = FromColor(col);
        h = temp.h;
        s = temp.s;
        b = temp.b;
        a = temp.a;
    }

    /// <summary>Converts <paramref name="color"/> into an <see cref="HSBColor"/> that represents it.</summary>
    /// <param name="color">The <see cref="Color"/> that needs to be converted.</param>
    /// <returns>An <see cref="HSBColor"/> represeting <paramref name="color"/>.</returns>
    public static HSBColor FromColor(Color color)
    {
        var ret = new HSBColor(0f, 0f, 0f, color.a);

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

            if (ret.h < 0)
                ret.h += 360f;
        }
        else
            ret.h = 0;

        ret.h *= 1f / 360f;
        ret.s = dif / max * 1f;
        ret.b = max;
        return ret;
    }

    /// <summary>Converts <paramref name="hsbColor"/> into a <see cref="Color"/> that represents it.</summary>
    /// <param name="hsbColor">The <see cref="HSBColor"/> that needs to be converted.</param>
    /// <returns>A <see cref="Color"/> represeting <paramref name="hsbColor"/>.</returns>
    public static Color ToColor(HSBColor hsbColor)
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

            if (h < 60f)
            {
                r = max;
                g = (h * dif / 60f) + min;
                b = min;
            }
            else if (h < 120f)
            {
                r = (-(h - 120f) * dif / 60f) + min;
                g = max;
                b = min;
            }
            else if (h < 180f)
            {
                r = min;
                g = max;
                b = ((h - 120f) * dif / 60f) + min;
            }
            else if (h < 240f)
            {
                r = min;
                g = (-(h - 240f) * dif / 60f) + min;
                b = max;
            }
            else if (h < 300f)
            {
                r = ((h - 240f) * dif / 60f) + min;
                g = min;
                b = max;
            }
            else if (h <= 360f)
            {
                r = max;
                g = min;
                b = (-(h - 360f) * dif / 60) + min;
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }
        }

        return new(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
    }

    /// <summary>Converts the current <see cref="HSBColor"/> into UnityEngine.<see cref="Color"/>.</summary>
    /// <returns>A UnityEngine.<see cref="Color"/> corresponding to the converted <see cref="HSBColor"/>.</returns>
    public readonly Color ToColor() => ToColor(this);

    /// <summary>Converts the current <see cref="HSBColor"/> into a <see cref="string"/> that represents it.</summary>
    /// <returns>A <see cref="string"/> represeting the <see cref="HSBColor"/>.</returns>
    public override readonly string ToString() => $"H: {h} S: {s} B: {b}";

    /// <summary>Finds an <see cref="HSBColor"/> at a certain point <paramref name="t"/> between two colors <paramref name="a"/> and <paramref name="b"/>.</summary>
    /// <param name="a">The <see cref="HSBColor"/> that sets one end.</param>
    /// <param name="b">The <see cref="HSBColor"/> that sets the other end.</param>
    /// <param name="t">The point between 0 and 1 that represents a certain <see cref="HSBColor"/> between <paramref name="a"/> and <paramref name="b"/>.</param>
    /// <returns>An <see cref="HSBColor"/> at a certain point <paramref name="t"/> between two colors <paramref name="a"/> and <paramref name="b"/>.</returns>
    public static HSBColor Lerp(HSBColor a, HSBColor b, float t)
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
            {
                var angle = Mathf.LerpAngle(a.h * 360f, b.h * 360f, t);

                while (angle < 0f)
                    angle += 360f;

                while (angle > 360f)
                    angle -= 360f;

                h = angle / 360f;
            }

            s = Mathf.Lerp(a.s, b.s, t);
        }

        return new(h, s, Mathf.Lerp(a.b, b.b, t), Mathf.Lerp(a.a, b.a, t));
    }

    /// <summary>Slides from one end of the <see cref="float"/> range to the other based on the <see cref="Time"/> that has passed.</summary>
    /// <param name="min">The lower end of the number range.</param>
    /// <param name="max">The higher end of the number range.</param>
    /// <param name="mul">How fast does the flipping happen.</param>
    /// <returns>A <see cref="float"/> between <paramref name="min"/> and <paramref name="max"/> based on the <see cref="Time"/> that has passed.</returns>
    public static float PingPong(float min, float max, float mul) => min + Mathf.PingPong(Time.time * mul, max - min);

    /// <summary>Slides from one end of the <see cref="float"/> range to the other based on the <see cref="Time"/> that has passed.</summary>
    /// <param name="min">The lower end of the number range.</param>
    /// <param name="max">The higher end of the number range.</param>
    /// <param name="mul">How fast does the flipping happen.</param>
    /// <returns>A <see cref="float"/> between <paramref name="min"/> and <paramref name="max"/> based on the <see cref="Time"/> that has passed.</returns>
    public static float PingPongReverse(float min, float max, float mul) => max - Mathf.PingPong(Time.time * mul, max - min);

    /// <summary>Parses the input text to an <see cref="HSBColor"/>.</summary>
    /// <param name="input">The input text.</param>
    /// <returns>An <see cref="HSBColor"/> based on the input.</returns>
    public static HSBColor Parse(string input)
    {
        input = input.Replace(" ", string.Empty);
        var parts = input.Split(';');

        if (parts.Length is not (3 or 4))
            throw new ArgumentOutOfRangeException(input);

        var parts2 = new List<float>();

        foreach (var part in parts)
        {
            var parts3 = part.Split(',');

            if (parts3.Length == 1)
                parts2.Add(float.Parse(parts3[0]));
            else if (parts3.Length == 2)
                parts2.Add(URandom.Range(float.Parse(parts3[0]), float.Parse(parts3[1])));
            else if (parts3.Length is 3 or 4)
            {
                var min = float.Parse(parts3[0]);
                var max = float.Parse(parts3[1]);
                var mul = float.Parse(parts3[2]);
                var reverse = parts.Length == 4 && bool.Parse(parts3[3]);
                parts2.Add(reverse ? PingPongReverse(min, max, mul) : PingPong(min, max, mul));
            }
            else
                throw new ArgumentOutOfRangeException(input);
        }

        return new(parts2[0], parts2[1], parts2[2], parts2.Count == 4 ? parts2[3] : 1f);
    }

    /// <summary>Tries to parse the input text to an <see cref="HSBColor"/>.</summary>
    /// <param name="input">The input text.</param>
    /// <param name="color">The resulting <see cref="HSBColor"/>.</param>
    /// <returns>true if the parse was successful; otherwise false.</returns>
    public static bool TryParse(string input, out HSBColor color)
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

    /*/// <summary>A test function to see if this actually works.</summary>
    public static void Test()
    {
        var color = new HSBColor(Color.red);
        Debug.Log("red: " + color);

        color = new(Color.green);
        Debug.Log("green: " + color);

        color = new(Color.blue);
        Debug.Log("blue: " + color);

        color = new(Color.grey);
        Debug.Log("grey: " + color);

        color = new(Color.white);
        Debug.Log("white: " + color);

        color = new(new(0.4f, 1f, 0.84f, 1f));
        Debug.Log("0.4, 1f, 0.84: " + color);

        Debug.Log("164,82,84   .... 0.643137f, 0.321568f, 0.329411f :" + ToColor(new(new(0.643137f, 0.321568f, 0.329411f))));
    }*/
}
