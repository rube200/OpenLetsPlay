using System.IO;

namespace OpenLetsPlay;

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
            Console.WriteLine($"Line '{line}'");
            var lineSplit = line.Trim().Split(' ', 2);
            Console.WriteLine($"Split1 '{lineSplit[0]}' '{lineSplit[0].Trim()}'");
            if (lineSplit.Length > 1)
            {
                Console.WriteLine($"Split2 '{lineSplit[1]}' '{lineSplit[1].Trim()}'");
            }
            
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