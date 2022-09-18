using System.IO;

namespace OpenLetsPlay.Controller;

public static class ConfigFileReader
{
    public static void ReadConfigFile()
    {
        var file = Parameters.ConfigFile;
        if (string.IsNullOrWhiteSpace(file))
            throw new NullReferenceException("ConfigFileReader: No config file provided");

        if (!Path.HasExtension(file))
            file = $"{file}.txt";

        using var streamReader = new StreamReader(file);
        while (streamReader.ReadLine() is { } line)
        {
            var lineSplit = line.Trim().Split(' ', 2);
            var arg = lineSplit[0].Trim();
            if (string.IsNullOrWhiteSpace(arg))
                continue;

            if (!Config.IsConfigParameter(arg))
            {
                Logger.LogWarn($"ConfigFileReader: Parameter({arg}) in file is invalid!");
                continue;
            }

            Config.SetParameter(arg, lineSplit.Length > 1 ? lineSplit[1].Trim() : true);
        }
    }
}