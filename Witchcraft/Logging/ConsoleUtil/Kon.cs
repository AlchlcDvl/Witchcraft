using System.Runtime.InteropServices;

namespace Witchcraft.ConsoleUtil;

internal class Kon
{
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleScreenBufferInfo(IntPtr hConsoleOutput, out CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, short attributes);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    private struct CONSOLE_SCREEN_BUFFER_INFO
    {
        internal COORD dwSize;
        internal COORD dwCursorPosition;
        internal short wAttributes;
        internal SMALL_RECT srWindow;
        internal COORD dwMaximumWindowSize;
    }

    private struct COORD
    {
        internal short X;
        internal short Y;
    }

    private struct SMALL_RECT
    {
        internal short Left;
        internal short Top;
        internal short Right;
        internal short Bottom;
    }

    private static readonly IntPtr INVALID_HANDLE_VALUE = new(-1);

    private static short ConsoleColorToColorAttribute(short color, bool isBackground)
    {
        if ((color & -16) != 0)
            throw new ArgumentException("Arg_InvalidConsoleColor");

        if (isBackground)
            color <<= 4;

        return color;
    }

    private static CONSOLE_SCREEN_BUFFER_INFO GetBufferInfo(bool throwOnNoConsole, out bool succeeded)
    {
        succeeded = false;

        if (!(conOut == INVALID_HANDLE_VALUE))
        {
            if (!GetConsoleScreenBufferInfo(conOut, out var console_SCREEN_BUFFER_INFO))
            {
                var consoleScreenBufferInfo = GetConsoleScreenBufferInfo(GetStdHandle(-12), out console_SCREEN_BUFFER_INFO);

                if (!consoleScreenBufferInfo)
                    consoleScreenBufferInfo = GetConsoleScreenBufferInfo(GetStdHandle(-10), out console_SCREEN_BUFFER_INFO);

                if (!consoleScreenBufferInfo)
                {
                    if (Marshal.GetLastWin32Error() == 6 && !throwOnNoConsole)
                        return default;
                }
            }

            succeeded = true;
            return console_SCREEN_BUFFER_INFO;
        }

        if (!throwOnNoConsole)
            return default;

        throw new Exception("IO.IO_NoConsole");
    }

    private static void SetConsoleColor(bool isBackground, ConsoleColor c)
    {
        var color = ConsoleColorToColorAttribute((short) c, isBackground);
        var bufferInfo = GetBufferInfo(false, out var flag);

        if (!flag)
            return;

        var num = bufferInfo.wAttributes;
        num &= (short)(isBackground ? -241 : -16);
        num = (short)((ushort)num | (ushort)color);
        SetConsoleTextAttribute(conOut, num);
    }

    private static ConsoleColor GetConsoleColor(bool isBackground)
    {
        var bufferInfo = GetBufferInfo(false, out var flag);

        if (!flag)
            return isBackground ? ConsoleColor.Black : ConsoleColor.Gray;

        return ColorAttributeToConsoleColor((short)(bufferInfo.wAttributes & 240));
    }

    private static ConsoleColor ColorAttributeToConsoleColor(short c)
    {
        if ((short)(c & 255) != 0)
            c >>= 4;

        return (ConsoleColor)c;
    }

    internal static IntPtr conOut = IntPtr.Zero;

    public static void ResetConsoleColor()
    {
        SetConsoleColor(true, ConsoleColor.Black);
        SetConsoleColor(false, ConsoleColor.Gray);
    }

    public static ConsoleColor ForegroundColor
    {
        get => GetConsoleColor(false);
        set => SetConsoleColor(false, value);
    }

    public static ConsoleColor BackgroundColor
    {
        get => GetConsoleColor(true);
        set => SetConsoleColor(true, value);
    }
}