using System.Text;
using Ageeml.Importer.Extensions;
using Ageeml.Importer.Models;
using Ageeml.Importer.Options;

namespace Ageeml.Importer.Services;

public sealed class PostgreSqlExporter : ISqlExporter
{
    private readonly OutputOptions _outputOptions;
    private readonly SqlLoader _sqlLoader;
    private readonly string _root;

    public PostgreSqlExporter(OutputOptions outputOptions, SqlLoader sqlLoader, string root)
    {
        _outputOptions = outputOptions;
        _sqlLoader = sqlLoader;
        _root = root;
    }

    public async Task ExportAsync(IReadOnlyList<Estado> estados, IReadOnlyList<Municipio> municipios, IReadOnlyList<Localidad> localidades)
    {
        var sqlPath = PathUtilities.ResolvePath(_root, _outputOptions.PostgreSqlSqlPath);
        var gzipPath = $"{sqlPath}.gz";
        Directory.CreateDirectory(Path.GetDirectoryName(sqlPath)!);

        await using (var writer = new StreamWriter(sqlPath, false, new UTF8Encoding(false)))
        {
            await writer.WriteLineAsync("DROP TABLE IF EXISTS localidades;");
            await writer.WriteLineAsync("DROP TABLE IF EXISTS municipios;");
            await writer.WriteLineAsync("DROP TABLE IF EXISTS estados;");
            await writer.WriteLineAsync();
            await writer.WriteLineAsync(_sqlLoader.Load("postgresql_estados.sql"));
            await writer.WriteLineAsync(_sqlLoader.Load("postgresql_municipios.sql"));
            await writer.WriteLineAsync(_sqlLoader.Load("postgresql_localidades.sql"));

            await FileUtilities.WriteInsertBatches(writer, "estados", "id, clave, nombre, abrev, activo", estados, e =>
                $"({e.Id}, {TextUtilities.Sql(e.Clave)}, {TextUtilities.Sql(e.Nombre)}, {TextUtilities.Sql(e.Abrev)}, 1)");

            await FileUtilities.WriteInsertBatches(writer, "municipios", "id, estado_id, clave, nombre, activo", municipios, m =>
                $"({m.Id}, {m.EstadoId}, {TextUtilities.Sql(m.Clave)}, {TextUtilities.Sql(m.Nombre)}, 1)");

            await FileUtilities.WriteInsertBatches(writer, "localidades", "id, municipio_id, clave, nombre, mapa, ambito, latitud, longitud, lat, lng, altitud, carta, poblacion, masculino, femenino, viviendas, activo", localidades, l =>
                $"({l.Id}, {l.MunicipioId}, {TextUtilities.Sql(l.Clave)}, {TextUtilities.Sql(l.Nombre)}, {l.Mapa}, {TextUtilities.Sql(l.Ambito)}, {TextUtilities.Sql(l.Latitud)}, {TextUtilities.Sql(l.Longitud)}, {TextUtilities.FormatDecimal(l.Lat)}, {TextUtilities.FormatDecimal(l.Lng)}, {TextUtilities.Sql(l.Altitud)}, {TextUtilities.Sql(l.Carta)}, {l.Poblacion}, {l.Masculino}, {l.Femenino}, {l.Viviendas}, 1)");

            await writer.WriteLineAsync("SELECT setval(pg_get_serial_sequence('estados','id'), (SELECT MAX(id) FROM estados));");
            await writer.WriteLineAsync("SELECT setval(pg_get_serial_sequence('municipios','id'), (SELECT MAX(id) FROM municipios));");
            await writer.WriteLineAsync("SELECT setval(pg_get_serial_sequence('localidades','id'), (SELECT MAX(id) FROM localidades));");
        }

        await FileUtilities.GzipAsync(sqlPath, gzipPath);
    }
}
