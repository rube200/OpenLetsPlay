using System.Collections.ObjectModel;
using System.Text;

namespace OpenLetsPlay;

public static class Parameters
{
    private static readonly IReadOnlyCollection<string> ConfigParameter = new[]
        { "Config", "ConfigFile", "Configuration", "ConfigurationFile" };

    public static string? ConfigFile { get; private set; }


    public static void ProcessArgs(IReadOnlyList<string> args)
    {
        if (args.Count == 0)
            throw new ArgumentException("Can not start! Arguments are empty! Use -help to list.");

        var help = false;
        var warningMessages = new Collection<string>();

        for (var i = 0; i < args.Count; i++)
        {
            var (arg, newParam) = args[i].CheckAndTrimStart("-");
            var nextIndex = i + 1;
            if (!newParam)
            {
                var order = new StringBuilder();
                if (i > 0)
                    order.Append($"{args[i - 1]} ");

                order.Append(arg);
                if (nextIndex < args.Count)
                    order.Append($" {args[nextIndex]}");

                warningMessages.Add($"{arg} is a parameter without '-' | Order: {order}");
                continue;
            }

            if (string.IsNullOrWhiteSpace(arg))
            {
                warningMessages.Add($"{args[i]} is not valid parameter");
                continue;
            }

            if (arg.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                help = true;
                continue;
            }

            var anyConfig = ConfigParameter.Any(cfg => cfg.Equals(arg, StringComparison.OrdinalIgnoreCase));
            if (anyConfig)
            {
                ConfigFile = args.GetParamValue(ref nextIndex);
                i = nextIndex;
                continue;
            }

            if (!Config.IsConfigParameter(arg))
            {
                warningMessages.Add($"{arg} is not a valid parameter");
                continue;
            }

            var parameter = args.GetParamValue(ref nextIndex);
            i = nextIndex;
            Config.SetParameter(arg, parameter.Length > 0 ? parameter : true);
        }

        if (help)
        {
            var usageBuilder = new StringBuilder();
            foreach (var (key, value) in Config.SelfComponents)
            {
                var tp = value.GetFpType();
                usageBuilder.Append($"{Environment.NewLine}\t-{key}: {tp.Name}");
            }

            throw new ArgumentException($"Usage: {usageBuilder}");
        }

        foreach (var warning in warningMessages)
            Logger.LogWarn($"Parameters: {warning}.");

        warningMessages.Clear();
    }

    private static string GetParamValue(this IReadOnlyList<string> args, ref int j)
    {
        var valueBuilder = new Collection<string>();
        for (; j < args.Count; j++)
        {
            var (nextArg, nextNewParam) = args[j].CheckAndTrimStart("-");
            if (nextNewParam || string.IsNullOrWhiteSpace(nextArg))
            {
                j--;
                break;
            }

            valueBuilder.Add(nextArg);
        }

        return string.Join(' ', valueBuilder);
    }
}