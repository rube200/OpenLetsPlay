using System.Reflection;

namespace OpenLetsPlay.Utils;

public class FieldAndProperty
{
    private static readonly FieldInfo ExMsg =
        typeof(Exception).GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private readonly FieldInfo? _field;
    private readonly PropertyInfo? _property;
    private readonly Type _type;
    public readonly string Name;

    public FieldAndProperty(FieldInfo field)
    {
        _field = field ?? throw new NullReferenceException($"FieldAndProperty: {nameof(field)} can not be null.");
        _type = field.FieldType;
        Name = field.Name;
    }

    public FieldAndProperty(PropertyInfo property)
    {
        _property = property ??
                    throw new NullReferenceException($"FieldAndProperty: {nameof(property)} can not be null.");
        _type = _property.PropertyType;
        Name = property.Name;
    }

    public Type GetFpType()
    {
        return (_field?.FieldType ?? _property?.PropertyType)!;
    }

    public void SetValue(object? value, object? instance = null)
    {
        try
        {
            var valueConverted = Convert.ChangeType(value, _type);
            _field?.SetValue(instance, valueConverted);
            _property?.SetValue(instance, valueConverted);
        }
        catch (Exception ex)
        {
            var msg = ExMsg.GetValue(ex) as string;
            ExMsg.SetValue(ex, $"{Name}({value}): {msg}");
            throw;
        }
    }
}