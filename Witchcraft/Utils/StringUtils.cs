using System.Text;

namespace Witchcraft.Utils;

/// <summary>A class for dealing with <see cref="string"/>s.</summary>
public static class StringUtils
{
    private const string ASCII = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ✧¢" +
        "乂⁂¤∮彡个「」人요〖〗ロ米卄王īl【】·ㅇ°◈◆◇◥◤◢◣《》︵︶☆☀☂☹☺♡♩♪♫♬✓☜☞☟☯☃✿❀÷º¿※⁑∞≠";

    /// <summary>Creates a string of random characters.</summary>
    /// <param name="maxLength">The length of the randomised string.</param>
    /// <returns>A randomised string.</returns>
    public static string GetRandomisedName(int maxLength)
    {
        var length = URandom.RandomRangeInt(1, maxLength + 1);
        var name = string.Empty;

        while (name.Length < length)
        {
            var random = URandom.RandomRangeInt(0, ASCII.Length);
            name += ASCII[random];
        }

        return name;
    }

    /// <summary>Wraps the text around a certain length to avoid text overflow out of the screen.</summary>
    /// <param name="text">The text to be wrapped.</param>
    /// <param name="width">The width of the wrap.</param>
    /// <param name="overflow">Decides if words can go beyond the width if the character limit is reached before the word ends. If <see langword="false"/>, the word is added into the next line instead.</param>
    /// <returns>A string representing the wrapped text, seperated by new lines.</returns>
    public static string WrapText(string text, int width = 90, bool overflow = true)
    {
        var result = new StringBuilder();
        var startIndex = 0;
        var column = 0;

        while (startIndex < text.Length)
        {
            var num = text.IndexOfAny(new char[] { ' ', '\t', '\r' }, startIndex);

            if (num != -1)
            {
                if (num == startIndex)
                    ++startIndex;
                else if (text[startIndex] == '\n')
                    startIndex++;
                else
                {
                    AddWord(text[startIndex..num]);
                    startIndex = num + 1;
                }
            }
            else
                break;
        }

        if (startIndex < text.Length)
            AddWord(text[startIndex..]);

        return result.ToString();

        void AddWord(string word)
        {
            var word1 = string.Empty;

            if (!overflow && word.Length > width)
            {
                for (var startIndex = 0; startIndex < word.Length; startIndex += word1.Length)
                {
                    word1 = word.Substring(startIndex, Math.Min(width, word.Length - startIndex));
                    AddWord(word1);
                }
            }
            else
            {
                if (column + word.Length >= width)
                {
                    if (column > 0)
                    {
                        result.AppendLine();
                        column = 0;
                    }
                }
                else if (column > 0)
                {
                    result.Append(' ');
                    column++;
                }

                result.Append(word);
                column += word.Length;
            }
        }
    }

    /// <summary>Wraps the texts around a certain length to avoid text overflow out of the screen. Each element is wrapped individually and added as a new line to the end result.</summary>
    /// <param name="texts">The texts to be wrapped.</param>
    /// <param name="width">The width of the wrap.</param>
    /// <param name="overflow">Decides if words can go beyond the width if the character limit is reached before the word ends. If <see langword="false"/>, the word is added into the next line instead.</param>
    /// <returns>A string representing the wrapped text, seperated by new lines.</returns>
    public static string WrapTexts(List<string> texts, int width = 90, bool overflow = true)
    {
        var result = WrapText(texts[0], width, overflow);
        texts.Skip(1).ForEach(x => result += $"\n{WrapText(x, width, overflow)}");
        return result;
    }

    /// <summary>Returns <paramref name="color"/> as a hexadecimal string in the format "RRGGBBAA".</summary>
    /// <param name="color">The color to be converted.</param>
    /// <returns>Hexadecimal string representing the color.</returns>
    /// <remarks>https://docs.unity3d.com/ScriptReference/ColorUtility.ToHtmlStringRGBA.html.</remarks>
    public static string ToHtmlStringRGBA(this Color32 color) => $"{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}";

    /// <inheritdoc cref="ToHtmlStringRGBA(Color32)"/>
    public static string ToHtmlStringRGBA(this Color color) => ((Color32)color).ToHtmlStringRGBA();
}
