namespace Witchcraft.Utils;

public static class StringUtils
{
    private const string ASCII = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ✧¢" +
        "乂⁂¤∮彡个「」人요〖〗ロ米卄王īl【】·ㅇ°◈◆◇◥◤◢◣《》︵︶☆☀☂☹☺♡♩♪♫♬✓☜☞☟☯☃✿❀÷º¿※⁑∞≠²";
    public static readonly char[] Lowercase = [ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' ];
    public static readonly char[] Uppercase = [ 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' ];

    public static string GetRandomisedString(int maxLength) => ASCII.GetRandomRange(URandom.RandomRangeInt(1, maxLength + 1)).ConstructString();

    public static string ConstructString(this IEnumerable<char> chars) => new([.. chars]);

    public static string WrapText(string text, int width = 90, bool overflow = true)
    {
        var result = new StringBuilder();
        var startIndex = 0;
        var column = 0;

        while (startIndex < text.Length)
        {
            var num = text.IndexOfAny([ ' ', '\t', '\r' ], startIndex);

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
            if (!overflow && word.Length > width)
            {
                string word1;

                for (var index = 0; index < word.Length; index += word1.Length)
                {
                    word1 = word.Substring(index, Math.Min(width, word.Length - index));
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

    public static string WrapTexts(IEnumerable<string> texts, int width = 90, bool overflow = true)
    {
        if (!texts.Any())
            return string.Empty;

        var result = WrapText(texts.FirstOrDefault(), width, overflow);
        texts.Skip(1).Do(x => result += $"\n{WrapText(x, width, overflow)}");
        return result;
    }

    public static string Repeat(this string str, int times)
    {
        if (times <= 0)
            return string.Empty;

        var append = str;

        for (var i = 0; i < times; i++)
            str += append;

        return str;
    }

    public static string AddSpaces(this string text)
    {
        Uppercase.Do(x =>
        {
            var index = text.IndexOf(x);

            if (index > 0)
                text = text.Insert(index, " ");
        });

        return text;
    }
}