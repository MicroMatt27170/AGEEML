using System.Globalization;

namespace Ageeml.Importer.Extensions;

public static class TextUtilities
{
    public static string Sql(string value) => $"'{value.Replace("'", "''")}'";

    public static string Pad(string? value, int length) =>
        TrimWhitespaceAndQuotes(value).PadLeft(length, '0');

    public static string Clean(string? value) =>
        TrimWhitespaceAndQuotes(value);

    public static int ParseInt(string? value) =>
        int.TryParse(TrimWhitespaceAndQuotes(value), NumberStyles.Any, CultureInfo.InvariantCulture, out var number) ? number : 0;

    public static double ParseDouble(string? value) =>
        double.TryParse(TrimWhitespaceAndQuotes(value), NumberStyles.Any, CultureInfo.InvariantCulture, out var number) ? number : 0d;

    public static string FormatDecimal(double value) =>
        value.ToString("0.#######", CultureInfo.InvariantCulture);

    private static string TrimWhitespaceAndQuotes(string? value) =>
        (value ?? string.Empty).Trim().Trim('"').Trim();
}
