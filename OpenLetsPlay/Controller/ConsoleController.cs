using System.Runtime.InteropServices;

namespace OpenLetsPlay.Controller;

public static class ConsoleController
{
    private const int SwHide = 0;
    private const int SwShow = 5;

    private static readonly IntPtr ConsoleWindows;
    private static bool _consoleVisible;

    static ConsoleController()
    {
        ConsoleWindows = GetConsoleWindow();
        _consoleVisible = true;
    }

    public static bool Enable
    {
        set
        {
            if (value == _consoleVisible)
                return;

            var type = value ? SwShow : SwHide;
            ShowWindow(ConsoleWindows, type);
            _consoleVisible = value;
        }
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
}