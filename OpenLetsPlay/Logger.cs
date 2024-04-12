using System;

namespace OpenLetsPlay;

public static class Logger
{
    public static void LogException(this Exception ex, string title)
    {
        LogImportant($"{ex}", title);
    }

    public static void LogException(this Exception ex, string message, string title)
    {
        LogImportant($"{message}:{Environment.NewLine}{ex}", title, message);
    }

    public static void LogInfo(string message)
    {
        LogConsole(message);
    }

    public static void LogWarn(string message)
    {
        LogConsole(message);
    }

    private static void LogConsole(string message)
    {
        Console.WriteLine(message);
    }

    public static void LogImportant(string message, string title, string? popupMessage = null)
    {
        Console.WriteLine(title);
        Console.WriteLine(popupMessage ?? message);
        Console.ReadLine();
    }
}