using System.Text;

namespace Witchcraft.Utils;

public static class Utility
{
    private const string Everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ✧¢" +
        "乂⁂¤∮彡个「」人요〖〗ロ米卄王īl【】·ㅇ°◈◆◇◥◤◢◣《》︵︶☆☀☂☹☺♡♩♪♫♬✓☜☞☟☯☃✿❀÷º¿※⁑∞≠";

    public static bool IsInRange(float num, float min, float max, bool minInclusive = false, bool maxInclusive = false)
    {
        if (minInclusive && maxInclusive)
            return num >= min && num <= max;
        else if (minInclusive)
            return num >= min && num < max;
        else if (maxInclusive)
            return num > min && num <= max;
        else
            return num > min && num < max;
    }

    public static string GetRandomisedName(int maxLength)
    {
        var length = URandom.RandomRangeInt(1, maxLength + 1);
        var name = "";

        while (name.Length < length)
        {
            var random = URandom.RandomRangeInt(0, Everything.Length);
            name += Everything[random];
        }

        return name;
    }

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
            var word1 = "";

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

    public static string WrapTexts(List<string> texts, int width = 90, bool overflow = true)
    {
        var result = WrapText(texts[0], width, overflow);
        texts.Skip(1).ForEach(x => result += $"\n{WrapText(x, width, overflow)}");
        return result;
    }

    public static bool IsNullEmptyOrWhiteSpace(string text) => text is null or "" || text.All(x => x == ' ') || text.Length == 0;

    public static void SaveText(string fileName, string textToSave, bool overrideText = false)
    {
        try
        {
            var text = Path.Combine(Witchcraft.ModPath, $"{fileName}-temp");
            var text2 = Path.Combine(Witchcraft.ModPath, fileName);
            var toOverride = overrideText ? "" : ReadText(fileName);
            File.WriteAllText(text, toOverride + textToSave);
            File.Delete(text2);
            File.Move(text, text2);
        }
        catch
        {
            Console.WriteLine($"Unable to save {textToSave} to {fileName}");
        }
    }

    public static string ReadText(string fileName)
    {
        try
        {
            return File.ReadAllText(Path.Combine(Witchcraft.ModPath, fileName));
        }
        catch
        {
            Console.WriteLine($"Error Loading {fileName}");
            return "";
        }
    }
}