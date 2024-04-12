using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OpenLetsPlay.Utils;

namespace OpenLetsPlay;

[Serializable]
public class Config
{
    public static readonly IReadOnlyDictionary<string, FieldAndProperty> SelfComponents;
    private static readonly Type SelfType = typeof(Config);
    private static Config? _instance;

    static Config()
    {
        var components = new Dictionary<string, FieldAndProperty>(StringComparer.OrdinalIgnoreCase);

        var fields = SelfType.GetFields(BindingFlags.Instance | BindingFlags.Public);
        foreach (var fp in fields.Select(p => new FieldAndProperty(p)))
        {
            components.Add(fp.Name, fp);
        }

        var properties = SelfType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var fp in properties.Select(p => new FieldAndProperty(p)))
        {
            components.Add(fp.Name, fp);
        }

        SelfComponents = components;
    }

    public static Config Instance => _instance ??= new Config();
    
    public static bool NoWaitGame => Instance.noWaitGame;
    public static bool Silent => Instance.silent;
    public static bool WaitExit => Instance.waitExit;
    public static bool WaitStart => Instance.waitStart;
    public static string? Arguments => Instance.arguments;
    public static string? GamePath => Instance.gamePath;
    public static string? GameDir => Instance.gameDir;

    public static bool IsConfigParameter(string parameter)
    {
        return SelfComponents.ContainsKey(parameter);
    }

    public static void SetParameter(string parameter, object value)
    {
        SelfComponents[parameter].SetValue(value, Instance);
    }

#pragma warning disable IDE1006
    // ReSharper disable InconsistentNaming
    public bool noWaitGame { get; private set; }
    public bool silent { get; private set; }
    public bool waitExit { get; private set; }
    public bool waitStart { get; private set; }
    public string? arguments { get; private set; }
    public string? gamePath { get; private set; }
    public string? gameDir { get; private set; }
    // ReSharper restore InconsistentNaming
#pragma warning restore IDE1006
}