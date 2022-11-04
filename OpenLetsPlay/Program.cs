using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using OpenLetsPlay.Controller;

namespace OpenLetsPlay;

public class Program
{
    private readonly ProcessStartInfo _processStartInfo;
    private Process? _process;

    private Program()
    {
        _processStartInfo = CreateProcessInfo();
        Console.Title = Path.GetFileName(_processStartInfo.FileName);
    }

    private static ProcessStartInfo CreateProcessInfo()
    {
        if (string.IsNullOrWhiteSpace(Config.GamePath))
            throw new NullReferenceException($"{nameof(Config.GamePath)} can not be null or empty.");

        var gameDir = Config.GameDir;
        if (string.IsNullOrWhiteSpace(gameDir))
            gameDir = Path.GetDirectoryName(Config.GamePath);

        return new ProcessStartInfo
        {
            Arguments = Config.Arguments,
            FileName = Config.GamePath,
            WorkingDirectory = gameDir,
            UseShellExecute = true
        };
    }

    private void Run()
    {
        if (Config.WaitStart)
            WaitInput("Game is ready to launch!");

        try
        {
            StartGame();
            CheckGame();
        }
        catch (Exception ex)
        {
            ex.LogException("Error while working with the game", "Game not started!");
            Environment.Exit(ex.HResult);
            return;
        }

        try
        {
            WaitGame();
        }
        catch (Exception ex)
        {
            ex.LogException("Game crash!");
            Environment.Exit(ex.HResult);
            return;
        }

        if (Config.WaitExit)
            WaitInput("Game terminated!");
    }

    private void StartGame()
    {
        Logger.LogInfo("Starting the game...");
        _process = Process.Start(_processStartInfo);
        Logger.LogInfo("Game started!");
    }

    private static void WaitInput(string msg)
    {
        void ShowPopup()
        {
            MessageBox.Show(msg, Console.Title, MessageBoxButton.OK);
        }

        if (!Config.Debug)
        {
            if (Config.Silent)
                return;

            ShowPopup();
            return;
        }

        if (Config.Silent)
        {
            Console.ReadLine();
            return;
        }

        var cancelToken = new CancellationTokenSource();
        var consoleTask = Task.Run(Console.ReadLine, cancelToken.Token);
        var msgBoxTask = Task.Run(ShowPopup, cancelToken.Token);

        Task.WaitAny(consoleTask, msgBoxTask);
        cancelToken.Cancel();
    }

    private void CheckGame()
    {
        if (_process is null)
            throw new NullReferenceException("Game did not start, process is null");

        Logger.LogInfo($"Game process started! {_process.ProcessName}({_process.Id}).");
    }

    private void WaitGame()
    {
        if (Config.NoWaitGame)
            return;

        Logger.LogInfo("Waiting for the game to stop...");
        _process!.WaitForExit();

        var exitCode = _process.ExitCode;
        _process.Dispose();

        if (exitCode != 0)
            throw new Exception($"Game crashed! Code. {exitCode}")
            {
                HResult = exitCode
            };

        Logger.LogInfo("Game stopped!");
    }

    public static void Main(string[] args)
    {
        Console.Title = "OpenLetsPlay";

        try
        {
            Parameters.ProcessArgs(args);
        }
        catch (Exception ex)
        {
            Logger.LogImportant(ex.Message, "No args or help");
            Environment.Exit(10);
            return;
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(Parameters.ConfigFile))
                ConfigFileReader.ReadConfigFile();
        }
        catch (Exception ex)
        {
            Logger.LogImportant(ex.Message, "Error config file");
            Environment.Exit(110);
            return;
        }

        if (Config.Debug)
        {
            var config = JsonSerializer.Serialize(Config.Instance, new JsonSerializerOptions { WriteIndented = true });
            Logger.LogDebug(config);
        }

        Program program;
        try
        {
            ConsoleController.Enable = Config.Debug;
            program = new Program();
        }
        catch (Exception ex)
        {
            ex.LogException($"Fail to setup: {ex.Message}", "Setup failed!");
            Environment.Exit(ex.HResult);
            return;
        }

        program.Run();
    }
}