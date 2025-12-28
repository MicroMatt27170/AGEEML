using System.Text;
using Ageeml.Importer.Extensions;
using Ageeml.Importer.Models;
using Ageeml.Importer.Options;
using Microsoft.Data.Sqlite;

namespace Ageeml.Importer.Services;

public sealed class SqliteExporter : ISqlExporter
{
    private readonly OutputOptions _outputOptions;
    private readonly SqlLoader _sqlLoader;
    private readonly string _root;

    public SqliteExporter(OutputOptions outputOptions, SqlLoader sqlLoader, string root)
    {
        _outputOptions = outputOptions;
        _sqlLoader = sqlLoader;
        _root = root;
    }

    public async Task ExportAsync(IReadOnlyList<Estado> estados, IReadOnlyList<Municipio> municipios, IReadOnlyList<Localidad> localidades)
    {
        var sqlPath = PathUtilities.ResolvePath(_root, _outputOptions.SqliteSqlPath);
        var gzipPath = $"{sqlPath}.gz";
        var dbPath = PathUtilities.ResolvePath(_root, _outputOptions.SqliteDbPath);
        Directory.CreateDirectory(Path.GetDirectoryName(sqlPath)!);

        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        await using (var writer = new StreamWriter(sqlPath, false, new UTF8Encoding(false)))
        {
            await writer.WriteLineAsync("PRAGMA foreign_keys=ON;");
            await writer.WriteLineAsync("DROP TABLE IF EXISTS localidades;");
            await writer.WriteLineAsync("DROP TABLE IF EXISTS municipios;");
            await writer.WriteLineAsync("DROP TABLE IF EXISTS estados;");
            await writer.WriteLineAsync();
            await writer.WriteLineAsync(_sqlLoader.Load("sqlite_estados.sql"));
            await writer.WriteLineAsync(_sqlLoader.Load("sqlite_municipios.sql"));
            await writer.WriteLineAsync(_sqlLoader.Load("sqlite_localidades.sql"));

            await FileUtilities.WriteInsertBatches(writer, "estados", "id, clave, nombre, abrev, activo", estados, e =>
                $"({e.Id}, {TextUtilities.Sql(e.Clave)}, {TextUtilities.Sql(e.Nombre)}, {TextUtilities.Sql(e.Abrev)}, 1)");

            await FileUtilities.WriteInsertBatches(writer, "municipios", "id, estado_id, clave, nombre, activo", municipios, m =>
                $"({m.Id}, {m.EstadoId}, {TextUtilities.Sql(m.Clave)}, {TextUtilities.Sql(m.Nombre)}, 1)");

            await FileUtilities.WriteInsertBatches(writer, "localidades", "id, municipio_id, clave, nombre, mapa, ambito, latitud, longitud, lat, lng, altitud, carta, poblacion, masculino, femenino, viviendas, activo", localidades, l =>
                $"({l.Id}, {l.MunicipioId}, {TextUtilities.Sql(l.Clave)}, {TextUtilities.Sql(l.Nombre)}, {l.Mapa}, {TextUtilities.Sql(l.Ambito)}, {TextUtilities.Sql(l.Latitud)}, {TextUtilities.Sql(l.Longitud)}, {TextUtilities.FormatDecimal(l.Lat)}, {TextUtilities.FormatDecimal(l.Lng)}, {TextUtilities.Sql(l.Altitud)}, {TextUtilities.Sql(l.Carta)}, {l.Poblacion}, {l.Masculino}, {l.Femenino}, {l.Viviendas}, 1)");
        }

        await FileUtilities.GzipAsync(sqlPath, gzipPath);

        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            ForeignKeys = true,
        }.ToString();

        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        SqliteUtilities.ExecuteNonQuery(connection, null, "PRAGMA journal_mode=OFF;");
        SqliteUtilities.ExecuteNonQuery(connection, null, "PRAGMA synchronous=OFF;");
        SqliteUtilities.ExecuteNonQuery(connection, null, "PRAGMA temp_store=MEMORY;");
        SqliteUtilities.ExecuteNonQuery(connection, null, "PRAGMA foreign_keys=ON;");

        using var transaction = connection.BeginTransaction();

        SqliteUtilities.ExecuteNonQuery(connection, transaction, "DROP TABLE IF EXISTS localidades;");
        SqliteUtilities.ExecuteNonQuery(connection, transaction, "DROP TABLE IF EXISTS municipios;");
        SqliteUtilities.ExecuteNonQuery(connection, transaction, "DROP TABLE IF EXISTS estados;");

        SqliteUtilities.ExecuteNonQuery(connection, transaction, _sqlLoader.Load("sqlite_estados.sql"));
        SqliteUtilities.ExecuteNonQuery(connection, transaction, _sqlLoader.Load("sqlite_municipios.sql"));
        SqliteUtilities.ExecuteNonQuery(connection, transaction, _sqlLoader.Load("sqlite_localidades.sql"));

        SqliteUtilities.InsertSqlite(connection, transaction, "INSERT INTO estados (id, clave, nombre, abrev, activo) VALUES ($id, $clave, $nombre, $abrev, 1);", estados, (cmd, e) =>
        {
            cmd.Parameters.AddWithValue("$id", e.Id);
            cmd.Parameters.AddWithValue("$clave", e.Clave);
            cmd.Parameters.AddWithValue("$nombre", e.Nombre);
            cmd.Parameters.AddWithValue("$abrev", e.Abrev);
        });

        SqliteUtilities.InsertSqlite(connection, transaction, "INSERT INTO municipios (id, estado_id, clave, nombre, activo) VALUES ($id, $estado_id, $clave, $nombre, 1);", municipios, (cmd, m) =>
        {
            cmd.Parameters.AddWithValue("$id", m.Id);
            cmd.Parameters.AddWithValue("$estado_id", m.EstadoId);
            cmd.Parameters.AddWithValue("$clave", m.Clave);
            cmd.Parameters.AddWithValue("$nombre", m.Nombre);
        });

        SqliteUtilities.InsertSqlite(connection, transaction, "INSERT INTO localidades (id, municipio_id, clave, nombre, mapa, ambito, latitud, longitud, lat, lng, altitud, carta, poblacion, masculino, femenino, viviendas, activo) VALUES ($id, $municipio_id, $clave, $nombre, $mapa, $ambito, $latitud, $longitud, $lat, $lng, $altitud, $carta, $poblacion, $masculino, $femenino, $viviendas, 1);", localidades, (cmd, l) =>
        {
            cmd.Parameters.AddWithValue("$id", l.Id);
            cmd.Parameters.AddWithValue("$municipio_id", l.MunicipioId);
            cmd.Parameters.AddWithValue("$clave", l.Clave);
            cmd.Parameters.AddWithValue("$nombre", l.Nombre);
            cmd.Parameters.AddWithValue("$mapa", l.Mapa);
            cmd.Parameters.AddWithValue("$ambito", l.Ambito);
            cmd.Parameters.AddWithValue("$latitud", l.Latitud);
            cmd.Parameters.AddWithValue("$longitud", l.Longitud);
            cmd.Parameters.AddWithValue("$lat", l.Lat);
            cmd.Parameters.AddWithValue("$lng", l.Lng);
            cmd.Parameters.AddWithValue("$altitud", l.Altitud);
            cmd.Parameters.AddWithValue("$carta", l.Carta);
            cmd.Parameters.AddWithValue("$poblacion", l.Poblacion);
            cmd.Parameters.AddWithValue("$masculino", l.Masculino);
            cmd.Parameters.AddWithValue("$femenino", l.Femenino);
            cmd.Parameters.AddWithValue("$viviendas", l.Viviendas);
        });

        transaction.Commit();
    }
}
