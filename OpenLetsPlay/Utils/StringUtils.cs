namespace OpenLetsPlay.Utils;

public static class StringUtils
{
    public static (string, bool) CheckAndTrimStart(this string value, string trimValue)
    {
        value = value.Trim();
        return value.StartsWith(trimValue, StringComparison.OrdinalIgnoreCase)
            ? (value.Substring(1, value.Length - 1).Trim(), true)
            : (value, false);
    }
}