using System.ComponentModel;
using System.Text;
using MonoMod.Utils;
using Witchcraft.Logging.Unix;
using Witchcraft.Logging.Windows;

namespace Witchcraft.Logging;

public enum ConsoleOutRedirectType
{
    [Description("Auto")]
    Auto = 0,

    [Description("Console Out")]
    ConsoleOut,

    [Description("Standard Out")]
    StandardOut
}

public static class ConsoleManager
{
    private const uint SHIFT_JIS_CP = 932;

    private const string ENABLE_CONSOLE_ARG = "--enable-console";

    private static readonly bool? EnableConsoleArgOverride;

    static ConsoleManager()
    {
        // Ensure GetCommandLineArgs failing (e.g. on unix) does not kill bepin
        try
        {
            var args = Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length; i++)
            {
                var res = args[i];
                if (res == ENABLE_CONSOLE_ARG && i + 1 < args.Length && bool.TryParse(args[i + 1], out var enable))
                    EnableConsoleArgOverride = enable;
            }
        }
        catch (Exception)
        {
            // Skip
        }
    }

    public static bool ConsoleEnabled => EnableConsoleArgOverride ?? Settings.LoggingActive;

    internal static IConsoleDriver Driver { get; set; }

    /// <summary>
    ///     True if an external console has been started, false otherwise.
    /// </summary>
    public static bool ConsoleActive => Driver?.ConsoleActive ?? false;

    /// <summary>
    ///     The stream that writes to the standard out stream of the process. Should never be null.
    /// </summary>
    public static TextWriter StandardOutStream => Driver?.StandardOut;

    /// <summary>
    ///     The stream that writes to an external console. Null if no such console exists
    /// </summary>
    public static TextWriter ConsoleStream => Driver?.ConsoleOut;


    public static void Initialize(bool alreadyActive, bool useManagedEncoder)
    {
        if (PlatformHelper.Is(MonoMod.Utils.Platform.Unix))
            Driver = new LinuxConsoleDriver();
        else if (PlatformHelper.Is(MonoMod.Utils.Platform.Windows))
            Driver = new WindowsConsoleDriver();
        else
            throw new PlatformNotSupportedException("Was unable to determine console driver for platform " + PlatformHelper.Current);

        Driver.Initialize(alreadyActive, useManagedEncoder);
    }

    private static void DriverCheck()
    {
        if (Driver == null)
            throw new InvalidOperationException("Driver has not been initialized");
    }

    public static void CreateConsole()
    {
        if (ConsoleActive)
            return;

        DriverCheck();

        // Apparently some versions of Mono throw a "Encoding name 'xxx' not supported"
        // if you use Encoding.GetEncoding
        // That's why we use of codepages directly and handle then in console drivers separately
        var codepage = Settings.ConsoleShiftJis ? SHIFT_JIS_CP : (uint)Encoding.UTF8.CodePage;

        Driver.CreateConsole(codepage);

        if (Settings.PreventClose)
            Driver.PreventClose();
    }

    public static void DetachConsole()
    {
        if (!ConsoleActive)
            return;

        DriverCheck();

        Driver.DetachConsole();
    }

    public static void SetConsoleTitle(string title)
    {
        DriverCheck();

        Driver.SetConsoleTitle(title);
    }

    public static void SetConsoleColor(ConsoleColor color)
    {
        DriverCheck();

        Driver.SetConsoleColor(color);
    }
}