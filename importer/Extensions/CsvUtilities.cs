using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Ageeml.Importer.Extensions;

public static class CsvUtilities
{
    public static CsvReader CreateCsvReader(TextReader reader)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Mode = CsvMode.NoEscape,
            BadDataFound = null,
            MissingFieldFound = null,
            TrimOptions = TrimOptions.Trim
        };

        return new CsvReader(reader, config);
    }
}
