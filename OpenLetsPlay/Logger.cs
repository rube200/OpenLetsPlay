using System;
using System.Windows;

namespace OpenLetsPlay;

public static class Logger
{
    // ReSharper disable once UnusedMember.Global
    public static void LogDebug(string message)
    {
        if (!Config.Debug)
            return;

        LogConsole(message);
    }

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
        Console.WriteLine(message);

        if (!Config.Silent)
            MessageBox.Show(popupMessage ?? message, $"{Console.Title}: {title}", MessageBoxButton.OK);
    }
}