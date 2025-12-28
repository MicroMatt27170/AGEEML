using Ageeml.Importer.Models;

namespace Ageeml.Importer.Services;

public interface ISqlExporter
{
    Task ExportAsync(IReadOnlyList<Estado> estados, IReadOnlyList<Municipio> municipios, IReadOnlyList<Localidad> localidades);
}
