using System.Net.Http;
using System.Text;
using Ageeml.Importer.Extensions;
using Ageeml.Importer.Models;
using Ageeml.Importer.Options;
using Ageeml.Importer.Services;

namespace Ageeml.Importer;

public sealed class AgeemlImporter
{
    private readonly HttpClient _httpClient = new();
    private readonly string _projectDir;
    private readonly DownloadOptions _downloadOptions;
    private readonly OutputOptions _outputOptions;
    private readonly SqlOptions _sqlOptions;
    private readonly string _root;
    private readonly string _workDir;
    private readonly SqlLoader _sqlLoader;
    private readonly IReadOnlyList<ISqlExporter> _exporters;

    public AgeemlImporter(string projectDir, DownloadOptions downloadOptions, OutputOptions outputOptions, SqlOptions sqlOptions)
    {
        _projectDir = projectDir;
        _downloadOptions = downloadOptions;
        _outputOptions = outputOptions;
        _sqlOptions = sqlOptions;

        _root = string.IsNullOrWhiteSpace(_outputOptions.RootPath)
            ? PathUtilities.FindRoot(_projectDir)
            : _outputOptions.RootPath;
        _workDir = PathUtilities.ResolvePath(_root, _outputOptions.WorkDir);

        var sqlsDir = PathUtilities.ResolvePath(_projectDir, _sqlOptions.SqlsDirectory);
        _sqlLoader = new SqlLoader(sqlsDir);

        _exporters = new ISqlExporter[]
        {
            new MySqlExporter(_outputOptions, _sqlLoader, _root),
            new PostgreSqlExporter(_outputOptions, _sqlLoader, _root),
            new SqliteExporter(_outputOptions, _sqlLoader, _root)
        };

        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("AgeemlImporter/1.0 (+https://github.com/developarts/AGEEML)");
    }

    public async Task RunAsync()
    {
        Console.WriteLine($"Usando raíz del repositorio: {_root}");
        Directory.CreateDirectory(_workDir);

        var estadoCsv = await DownloadAndExtractAsync(_downloadOptions.EstadosUrl, "entidades");
        var municipioCsv = await DownloadAndExtractAsync(_downloadOptions.MunicipiosUrl, "municipios");
        var localidadCsv = await DownloadAndExtractAsync(_downloadOptions.LocalidadesUrl, "localidades");

        var estados = LoadEstados(estadoCsv);
        var estadoLookup = estados.ToDictionary(e => e.Clave, e => e);

        var (municipios, municipioLookup) = LoadMunicipios(municipioCsv, estadoLookup);

        var localidades = LoadLocalidades(localidadCsv, municipioLookup);

        Console.WriteLine($"Estados: {estados.Count}, Municipios: {municipios.Count}, Localidades: {localidades.Count}");

        foreach (var exporter in _exporters)
        {
            await exporter.ExportAsync(estados, municipios, localidades);
        }

        Console.WriteLine("Importación finalizada.");
    }

    private async Task<string> DownloadAndExtractAsync(string url, string name)
    {
        var zipPath = Path.Combine(_workDir, $"{name}.zip");
        var extractDir = Path.Combine(_workDir, name);

        if (Directory.Exists(extractDir))
        {
            Directory.Delete(extractDir, true);
        }

        Console.WriteLine($"Descargando {name} desde {url}");
        using (var response = await _httpClient.GetAsync(url))
        {
            response.EnsureSuccessStatusCode();
            await using var fs = File.Create(zipPath);
            await response.Content.CopyToAsync(fs);
        }

        if (!FileUtilities.LooksLikeZip(zipPath))
        {
            throw new InvalidDataException($"El archivo descargado desde {url} no es un ZIP válido.");
        }

        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath, extractDir);

        var csvFile = Directory.EnumerateFiles(extractDir, "*.csv", SearchOption.AllDirectories)
            .FirstOrDefault(f => !Path.GetFileName(f).Contains("utf", StringComparison.OrdinalIgnoreCase));

        if (csvFile is null)
        {
            throw new InvalidOperationException($"No se encontró CSV válido en {extractDir}");
        }

        return csvFile;
    }

    private static List<Estado> LoadEstados(string csvPath)
    {
        using var reader = new StreamReader(csvPath, Encoding.Latin1);
        using var csv = CsvUtilities.CreateCsvReader(reader);
        csv.Read();
        csv.ReadHeader();

        var estados = new List<Estado>();
        while (csv.Read())
        {
            var clave = TextUtilities.Pad(csv.GetField("CVE_ENT"), 2);
            var nombre = TextUtilities.Clean(csv.GetField("NOM_ENT"));
            var abrev = TextUtilities.Clean(csv.GetField("NOM_ABR"));

            estados.Add(new Estado
            {
                Clave = clave,
                Nombre = nombre,
                Abrev = abrev
            });
        }

        return estados
            .OrderBy(e => e.Clave, StringComparer.Ordinal)
            .Select((e, index) =>
            {
                e.Id = index + 1;
                return e;
            })
            .ToList();
    }

    private static (List<Municipio> Municipios, Dictionary<string, Municipio> MunicipioLookup) LoadMunicipios(
        string csvPath,
        IReadOnlyDictionary<string, Estado> estados)
    {
        using var reader = new StreamReader(csvPath, Encoding.Latin1);
        using var csv = CsvUtilities.CreateCsvReader(reader);
        csv.Read();
        csv.ReadHeader();

        var municipios = new List<Municipio>();
        var municipioLookup = new Dictionary<string, Municipio>(StringComparer.Ordinal);
        while (csv.Read())
        {
            var claveEstado = TextUtilities.Pad(csv.GetField("CVE_ENT"), 2);
            if (!estados.TryGetValue(claveEstado, out var estado))
            {
                throw new InvalidOperationException($"Estado no encontrado para clave {claveEstado}");
            }

            var clave = TextUtilities.Pad(csv.GetField("CVE_MUN"), 3);
            var nombre = TextUtilities.Clean(csv.GetField("NOM_MUN"));

            municipios.Add(new Municipio
            {
                EstadoId = estado.Id,
                Clave = clave,
                Nombre = nombre
            });

            municipioLookup[$"{claveEstado}{clave}"] = municipios[^1];
        }

        var ordered = municipios
            .OrderBy(m => m.EstadoId)
            .ThenBy(m => m.Clave, StringComparer.Ordinal)
            .Select((m, index) =>
            {
                m.Id = index + 1;
                return m;
            })
            .ToList();

        return (ordered, municipioLookup);
    }

    private static List<Localidad> LoadLocalidades(string csvPath, IReadOnlyDictionary<string, Municipio> municipios)
    {
        using var reader = new StreamReader(csvPath, Encoding.Latin1);
        using var csv = CsvUtilities.CreateCsvReader(reader);
        csv.Read();
        csv.ReadHeader();

        var localidades = new List<Localidad>(310_000);
        while (csv.Read())
        {
            var claveEstado = TextUtilities.Pad(csv.GetField("CVE_ENT"), 2);
            var claveMunicipio = TextUtilities.Pad(csv.GetField("CVE_MUN"), 3);
            var claveLocalidad = TextUtilities.Pad(csv.GetField("CVE_LOC"), 4);
            var municipioKey = $"{claveEstado}{claveMunicipio}";

            if (!municipios.TryGetValue(municipioKey, out var municipio))
            {
                throw new InvalidOperationException($"Municipio no encontrado para clave {municipioKey}");
            }

            var mapa = TextUtilities.ParseInt(csv.GetField("CVEGEO"));
            var ambito = TextUtilities.Clean(csv.GetField("AMBITO"));
            var latitud = TextUtilities.Clean(csv.GetField("LATITUD"));
            var longitud = TextUtilities.Clean(csv.GetField("LONGITUD"));
            var lat = TextUtilities.ParseDouble(csv.GetField("LAT_DECIMAL"));
            var lng = TextUtilities.ParseDouble(csv.GetField("LON_DECIMAL"));
            var altitud = TextUtilities.Clean(csv.GetField("ALTITUD"));
            var carta = TextUtilities.Clean(csv.GetField("CVE_CARTA"));
            var poblacion = TextUtilities.ParseInt(csv.GetField("POB_TOTAL"));
            var masculino = TextUtilities.ParseInt(csv.GetField("POB_MASCULINA"));
            var femenino = TextUtilities.ParseInt(csv.GetField("POB_FEMENINA"));
            var viviendas = TextUtilities.ParseInt(csv.GetField("TOTAL DE VIVIENDAS HABITADAS"));
            var nombre = TextUtilities.Clean(csv.GetField("NOM_LOC"));

            localidades.Add(new Localidad
            {
                Clave = claveLocalidad,
                MunicipioId = municipio.Id,
                Nombre = nombre,
                Mapa = mapa,
                Ambito = ambito,
                Latitud = latitud,
                Longitud = longitud,
                Lat = lat,
                Lng = lng,
                Altitud = altitud,
                Carta = carta,
                Poblacion = poblacion,
                Masculino = masculino,
                Femenino = femenino,
                Viviendas = viviendas
            });
        }

        return localidades
            .OrderBy(l => l.MunicipioId)
            .ThenBy(l => l.Clave, StringComparer.Ordinal)
            .Select((l, index) =>
            {
                l.Id = index + 1;
                return l;
            })
            .ToList();
    }
}
