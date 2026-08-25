using System.Globalization;

namespace Solentik.Paystack.Internal;

internal static class RequestUtilities
{
    public static string EscapeRequired(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }

        return Uri.EscapeDataString(value);
    }

    public static void Add(List<KeyValuePair<string, string>> query, string key, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            query.Add(new(key, value));
        }
    }

    public static void AddRequired(List<KeyValuePair<string, string>> query, string key, string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("The value cannot be empty.", parameterName);
        }

        query.Add(new(key, value));
    }

    public static void AddBool(List<KeyValuePair<string, string>> query, string key, bool? value)
    {
        if (value is not null)
        {
            query.Add(new(key, value.Value ? "true" : "false"));
        }
    }

    public static void AddPositive(List<KeyValuePair<string, string>> query, string key, int? value)
    {
        if (value is null)
        {
            return;
        }

        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(key, "The value must be greater than zero.");
        }

        query.Add(new(key, value.Value.ToString(CultureInfo.InvariantCulture)));
    }

    public static string WithQuery(string path, IEnumerable<KeyValuePair<string, string>> query)
    {
        var values = query.ToArray();
        return values.Length == 0
            ? path
            : $"{path}?{string.Join("&", values.Select(pair =>
                $"{Uri.EscapeDataString(pair.Key)}={Uri.EscapeDataString(pair.Value)}"))}";
    }
}
