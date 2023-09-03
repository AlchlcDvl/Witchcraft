namespace Witchcraft.Logging;

[Flags]
public enum LogLevel
{
    None = 0,
    Fatal = 1,
    Critical = 2,
    Error = 4,
    Warning = 8,
    Info = 16,
    All = Fatal | Critical | Error | Warning | Info
}