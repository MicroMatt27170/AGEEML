using System.Text;

namespace Ageeml.Importer.Extensions;

public sealed class SqlLoader
{
    private readonly string _sqlsDir;

    public SqlLoader(string sqlsDir)
    {
        _sqlsDir = sqlsDir;
    }

    public string Load(string fileName)
    {
        var path = Path.Combine(_sqlsDir, fileName);
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"No se encontró el archivo SQL: {path}");
        }

        return File.ReadAllText(path, Encoding.UTF8);
    }
}
