using System.Reflection;

namespace OpenLetsPlay;

public class FieldAndProperty
{
    private static readonly FieldInfo ExMsg =
        typeof(Exception).GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private readonly FieldInfo? _field;
    private readonly PropertyInfo? _property;
    public readonly string Name;
#pragma warning disable CS0649
    private object? _instance;
#pragma warning restore CS0649

    public FieldAndProperty(FieldInfo field)
    {
        _field = field ?? throw new NullReferenceException($"FieldAndProperty: {nameof(field)} can not be null.");
        Name = field.Name;
    }

    /*public FieldAndProperty(FieldInfo field, object instance) : this(field)
    {
        _instance = instance;
    }*/

    public FieldAndProperty(PropertyInfo property)
    {
        _property = property ??
                    throw new NullReferenceException($"FieldAndProperty: {nameof(property)} can not be null.");
        Name = property.Name;
    }

    /*public void SetInstance(object instance)
    {
        if (_instance == instance)
            return;
        
        if (_instance is not null)
            throw new ReadOnlyException($"FieldAndProperty: Instance is already defined.");

        _instance = instance;
    }
    
    public FieldAndProperty(PropertyInfo property, object instance) : this(property)
    {
        _instance = instance;
    }*/

    public Type GetFpType()
    {
        return (_field?.FieldType ?? _property?.PropertyType)!;
    }
    /*
    public object? GetValue(object? instance = null)
    {
        //todo update, incorrect behaviour
        return _field?.GetValue(_instance ?? instance) ?? _property?.GetValue(_instance ?? instance);
    }*/

    public void SetValue(object? value, object? instance = null)
    {
        try
        {
            _field?.SetValue(_instance ?? instance, value);
            _property?.SetValue(_instance ?? instance, value);
        }
        catch (Exception ex)
        {
            var msg = ExMsg.GetValue(ex) as string;
            ExMsg.SetValue(ex, $"{Name}({value}): {msg}");
            throw;
        }
    }
}